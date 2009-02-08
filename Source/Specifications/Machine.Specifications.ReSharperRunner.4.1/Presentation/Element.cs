using System;
using System.Collections.Generic;

using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal abstract class Element : UnitTestElement
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly string _typeName;

    protected Element(IUnitTestProvider provider,
                      UnitTestElement parent,
                      IProjectModelElement project,
                      string typeName,
                      bool isIgnored)
      : base(provider, parent)
    {
      if (project == null && !Shell.Instance.IsTestShell)
      {
        throw new ArgumentNullException("project");
      }

      if (typeName == null)
      {
        throw new ArgumentNullException("typeName");
      }

      if (project != null)
      {
        _projectEnvoy = new ProjectModelElementEnvoy(project);
      }

      _typeName = typeName;

      if (isIgnored)
      {
        SetExplicit("Ignored");
      }
    }

    public override IProject GetProject()
    {
      return _projectEnvoy.GetValidProjectElement() as IProject;
    }

    protected ITypeElement GetDeclaredType()
    {
      IProject project = GetProject();
      if (project == null)
      {
        return null;
      }

      PsiManager manager = PsiManager.GetInstance(project.GetSolution());
      using (ReadLockCookie.Create())
      {
        DeclarationsCacheScope scope = DeclarationsCacheScope.ProjectScope(project, true);
        IDeclarationsCache declarationsCache = manager.GetDeclarationsCache(scope, true);
        return declarationsCache.GetTypeElementByCLRName(_typeName);
      }
    }

    public override string GetTypeClrName()
    {
      return _typeName;
    }

    public override UnitTestNamespace GetNamespace()
    {
      return new UnitTestNamespace(new CLRTypeName(_typeName).NamespaceName);
    }

    public override IList<IProjectFile> GetProjectFiles()
    {
      ITypeElement declaredType = GetDeclaredType();
      if (declaredType == null)
      {
        return EmptyArray<IProjectFile>.Instance;
      }

      return declaredType.GetProjectFiles();
    }

    public override UnitTestElementDisposition GetDisposition()
    {
      IDeclaredElement element = GetDeclaredElement();
      if (element == null || !element.IsValid())
      {
        return UnitTestElementDisposition.ourInvalidDisposition;
      }

      // TODO: Use map higher order function.
      var locations = new List<UnitTestElementLocation>();
      element.GetDeclarations().ForEach(declaration =>
        {
          IFile file = declaration.GetContainingFile();
          if (file != null)
          {
            locations.Add(new UnitTestElementLocation(file.ProjectFile,
                                                      declaration.GetNameRange(),
                                                      declaration.GetDocumentRange().TextRange));
          }
        });

      return new UnitTestElementDisposition(locations, this);
    }

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
      {
        Element element = (Element) obj;
        return Equals(element._projectEnvoy, _projectEnvoy) && element._typeName == _typeName;
      }

      return false;
    }

    public override int GetHashCode()
    {
      int result = base.GetHashCode();
      result = 29 * result + _projectEnvoy.GetHashCode();
      result = 29 * result + _typeName.GetHashCode();
      return result;
    }
  }
}