using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
    abstract class Element : IUnitTestElement
    {
        readonly MSpecUnitTestProvider _provider;
        readonly string _declaringTypeName;
        readonly ProjectModelElementEnvoy _projectEnvoy;
        readonly UnitTestTaskFactory _taskFactory;

        protected Element(MSpecUnitTestProvider provider,
                          Element parent,
                          ProjectModelElementEnvoy projectEnvoy,
                          string declaringTypeName,
                          bool isIgnored)
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

        public string Id
        {
            get { return TypeName; }
        }

        public IUnitTestProvider Provider
        {
            get { return _provider; }
        }

        public IUnitTestElement Parent { get; set; }
        public ICollection<IUnitTestElement> Children { get; private set; }
        public abstract string ShortName { get; }

        public bool Explicit
        {
            get { return false; }
        }

        public UnitTestElementState State { get; set; }

        public virtual string GetTitlePrefix()
        {
            return String.Empty;
        }

        public IProject GetProject()
        {
            return _projectEnvoy.GetValidProjectElement() as IProject;
        }

        protected ITypeElement GetDeclaredType()
        {
            var project = GetProject();
            if (project == null)
            {
                return null;
            }

            var psiModule = _provider.PsiModuleManager.GetPrimaryPsiModule(project);
            if (psiModule == null)
            {
                return null;
            }

            var declarationsCache = _provider.CacheManager.GetDeclarationsCache(psiModule, true, true);
            return declarationsCache.GetTypeElementByCLRName(_declaringTypeName);
        }

        public string GetTypeClrName()
        {
            return _declaringTypeName;
        }

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(new ClrTypeName(_declaringTypeName).GetNamespaceName());
        }

        public UnitTestElementDisposition GetDisposition()
        {
            var element = GetDeclaredElement();
            if (element == null || !element.IsValid())
            {
                return UnitTestElementDisposition.InvalidDisposition;
            }

            var locations = new List<UnitTestElementLocation>();
            element.GetDeclarations().ForEach(declaration =>
            {
                var file = declaration.GetContainingFile();
                if (file != null)
                {
                    locations.Add(new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(),
                                                              declaration.GetNameDocumentRange().TextRange,
                                                              declaration.GetDocumentRange().TextRange));
                }
            });

            return new UnitTestElementDisposition(locations, this);
        }

        public override bool Equals(object obj)
        {
            var other = (Element) obj;
            return Equals(other._projectEnvoy, _projectEnvoy) && other._declaringTypeName == _declaringTypeName;
        }

        public override int GetHashCode()
        {
            var result = 0;
            result = 29 * result + _projectEnvoy.GetHashCode();
            result = 29 * result + _declaringTypeName.GetHashCode();
            return result;
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
                return other.ShortName == ShortName && other.Provider == Provider
                       && Equals(element._projectEnvoy, _projectEnvoy) &&
                       element._declaringTypeName == _declaringTypeName;
            }
            return false;
        }

        public abstract string GetPresentation();
        public abstract IDeclaredElement GetDeclaredElement();

        public IEnumerable<IProjectFile> GetProjectFiles() {
            ITypeElement declaredType = GetDeclaredType();
            if (declaredType == null) {
                return EmptyArray<IProjectFile>.Instance;
            }

            return declaredType.GetSourceFiles().Select(x => x.ToProjectFile());
        }

        public IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
        {
            if (this is ContextSpecificationElement)
            {
                var contextSpecification = this as ContextSpecificationElement;
                var context = contextSpecification.Context;

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
                var context = behavior.Context;

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
                var behavior = behaviorSpecification.Behavior;
                var context = behavior.Context;

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
    }
}