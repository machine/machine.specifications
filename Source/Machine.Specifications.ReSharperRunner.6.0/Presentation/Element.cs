using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public abstract class Element : IUnitTestElement
  {
    readonly string _declaringTypeName;
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly UnitTestTaskFactory _taskFactory;
    Element _parent;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;

    protected Element(MSpecUnitTestProvider provider,
                      PsiModuleManager psiModuleManager,
                      CacheManager cacheManager,
                      Element parent,
                      ProjectModelElementEnvoy projectEnvoy, string declaringTypeName, bool isIgnored)
    {
      if (projectEnvoy == null && !Shell.Instance.IsTestShell)
      {
        throw new ArgumentNullException("project");
      }

      if (declaringTypeName == null)
      {
        throw new ArgumentNullException("declaringTypeName");
      }

      if (projectEnvoy != null)
      {
        _projectEnvoy = projectEnvoy;
      }

      _provider = provider;
      _declaringTypeName = declaringTypeName;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;

      if (isIgnored)
      {
        ExplicitReason = "Ignored";
      }

      TypeName = declaringTypeName;
      Parent = parent;

      Children = new List<IUnitTestElement>();
      State = UnitTestElementState.Valid;
      _taskFactory = new UnitTestTaskFactory(_provider.ID);
    }

    public string TypeName { get; protected set; }
    public abstract string Kind { get; }
    public abstract IEnumerable<UnitTestElementCategory> Categories { get; }
    public string ExplicitReason { get; private set; }

    public abstract string Id
    {
      get;
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }

    public IUnitTestElement Parent
    {
        get { return _parent;  }
        set
        {
          if (_parent == value)
          {
            return;
          }

          if (_parent != null)
          {
            _parent.RemoveChild(this);
          }

          _parent = (Element)value;
          if (_parent != null)
          {
            _parent.AddChild(this);
          }
        }
    }
    public ICollection<IUnitTestElement> Children { get; private set; }
    public abstract string ShortName { get; }

    public bool Explicit
    {
      get { return false; }
    }

    public UnitTestElementState State { get; set; }

    public IProject GetProject()
    {
      return _projectEnvoy.GetValidProjectElement() as IProject;
    }

    public UnitTestNamespace GetNamespace()
    {
      return new UnitTestNamespace(new ClrTypeName(_declaringTypeName).GetNamespaceName());
    }

    public UnitTestElementDisposition GetDisposition()
    {
      IDeclaredElement element = GetDeclaredElement();
      if (element == null || !element.IsValid())
      {
        return UnitTestElementDisposition.InvalidDisposition;
      }

      var locations = new List<UnitTestElementLocation>();
      element.GetDeclarations().ForEach(declaration =>
      {
        IFile file = declaration.GetContainingFile();
        if (file != null)
        {
          locations.Add(new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(),
                                                    declaration.GetNameDocumentRange().TextRange,
                                                    declaration.GetDocumentRange().TextRange));
        }
      });

      return new UnitTestElementDisposition(locations, this);
    }

    public bool Equals(IUnitTestElement other)
    {
      if (ReferenceEquals(this, other))
      {
        return true;
      }

      if (other.GetType() == GetType())
      {
        var element = (Element) other;
        return other.ShortName == ShortName
               && other.Provider == Provider
               && Equals(element._projectEnvoy, _projectEnvoy)
               && element._declaringTypeName == _declaringTypeName;
      }
      return false;
    }

    public abstract string GetPresentation();

    public string GetPresentation(IUnitTestElement unitTestElement)
    {
        return GetPresentation();
    }

    public abstract IDeclaredElement GetDeclaredElement();

    public IEnumerable<IProjectFile> GetProjectFiles()
    {
      ITypeElement declaredType = GetDeclaredType();
      if (declaredType == null)
      {
        return EmptyArray<IProjectFile>.Instance;
      }

      return declaredType.GetSourceFiles().Select(x => x.ToProjectFile());
    }

#if RESHARPER_7
    public IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch unitTestLaunch)
#elif RESHARPER_61
	 public IList<UnitTestTask> GetTaskSequence(IList<IUnitTestElement> explicitElements)
#else
    public IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
#endif
    {
      if (this is ContextSpecificationElement)
      {
        var contextSpecification = this as ContextSpecificationElement;
        ContextElement context = contextSpecification.Context;

        return new List<UnitTestTask>
               {
                 _taskFactory.CreateAssemblyLoadTask(context),
                 _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
                 _taskFactory.CreateContextSpecificationTask(context,
                                                             contextSpecification,
                                                             explicitElements.Contains(contextSpecification))
               };
      }

      if (this is BehaviorElement)
      {
        var behavior = this as BehaviorElement;
        ContextElement context = behavior.Context;

        return new List<UnitTestTask>
               {
                 _taskFactory.CreateAssemblyLoadTask(context),
                 _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
                 _taskFactory.CreateBehaviorTask(context, behavior, explicitElements.Contains(behavior))
               };
      }

      if (this is BehaviorSpecificationElement)
      {
        var behaviorSpecification = this as BehaviorSpecificationElement;
        BehaviorElement behavior = behaviorSpecification.Behavior;
        ContextElement context = behavior.Context;

        return new List<UnitTestTask>
               {
                 _taskFactory.CreateAssemblyLoadTask(context),
                 _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
                 _taskFactory.CreateBehaviorTask(context, behavior, explicitElements.Contains(behavior)),
                 _taskFactory.CreateBehaviorSpecificationTask(context,
                                                              behaviorSpecification,
                                                              explicitElements.Contains(behaviorSpecification))
               };
      }

      if (this is ContextElement)
      {
        return EmptyArray<UnitTestTask>.Instance;
      }

      throw new ArgumentException(String.Format("Element is not a Machine.Specifications element: '{0}'", this));
    }

    public virtual string GetTitlePrefix()
    {
      return String.Empty;
    }

    protected ITypeElement GetDeclaredType()
    {
      IProject project = GetProject();
      if (project == null)
      {
        return null;
      }

#if RESHARPER_61
      IPsiModule psiModule = _psiModuleManager.GetPrimaryPsiModule(project);
      if (psiModule == null)
      {
        return null;
      }

      IDeclarationsCache declarationsCache = _cacheManager.GetDeclarationsCache(psiModule, true, true);
#else
      IPsiModule psiModule = _provider.PsiModuleManager.GetPrimaryPsiModule(project);
      if (psiModule == null)
      {
        return null;
      }

      IDeclarationsCache declarationsCache = _provider.CacheManager.GetDeclarationsCache(psiModule, true, true);
#endif
      return declarationsCache.GetTypeElementByCLRName(_declaringTypeName);
    }

    public string GetTypeClrName()
    {
      return _declaringTypeName;
    }

    public override bool Equals(object obj)
    {
      var other = obj as Element;
      if (other == null)
      {
        return false;
      }

      return Equals(other._projectEnvoy, _projectEnvoy) && other.Id == Id;
    }

    public override int GetHashCode()
    {
      int result = 0;
      result = 29 * result + _projectEnvoy.GetHashCode();
      result = 29 * result + Id.GetHashCode();
      return result;
    }

    void AddChild(Element behaviorElement)
    {
      Children.Add(behaviorElement);
    }

    void RemoveChild(Element behaviorElement)
    {
      Children.Remove(behaviorElement);
    }
  }
}