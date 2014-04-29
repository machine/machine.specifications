namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.TaskRunnerFramework;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Strategy;
    using JetBrains.Util;

    using Machine.Specifications.ReSharperProvider.Factories;
    using Machine.Specifications.ReSharperProvider.Shims;
    using Machine.Specifications.ReSharperRunner;

    public abstract class Element : IUnitTestElement
    {
        readonly IClrTypeName _declaringTypeName;
        readonly ProjectModelElementEnvoy _projectEnvoy;
        readonly MSpecUnitTestProvider _provider;
        readonly UnitTestTaskFactory _taskFactory;
        Element _parent;
        readonly IPsi _psiModuleManager;
        readonly ICache _cacheManager;

        protected Element(MSpecUnitTestProvider provider,
                          IPsi psiModuleManager,
                          ICache cacheManager,
                          Element parent,
                          ProjectModelElementEnvoy projectEnvoy,
                          IClrTypeName declaringTypeName,
                          bool isIgnored)
        {
            if (declaringTypeName == null)
            {
                throw new ArgumentNullException("declaringTypeName");
            }

            if (projectEnvoy != null)
            {
                this._projectEnvoy = projectEnvoy;
            }

            this._provider = provider;
            this._declaringTypeName = declaringTypeName;
            this._psiModuleManager = psiModuleManager;
            this._cacheManager = cacheManager;

            if (isIgnored)
            {
                this.ExplicitReason = "Ignored";
            }

            this.TypeName = declaringTypeName;
            this.Parent = parent;

            this.Children = new List<IUnitTestElement>();
            this.State = UnitTestElementState.Valid;
            this._taskFactory = new UnitTestTaskFactory(this._provider.ID);
        }

        public IClrTypeName TypeName { get; private set; }

        public abstract string Kind { get; }
        public abstract IEnumerable<UnitTestElementCategory> Categories { get; }
        public string ExplicitReason { get; private set; }

        public abstract string Id
        {
            get;
        }

        public IUnitTestProvider Provider
        {
            get { return this._provider; }
        }

        public IUnitTestElement Parent
        {
            get { return this._parent; }
            set
            {
                if (this._parent == value)
                {
                    return;
                }

                if (this._parent != null)
                {
                    this._parent.RemoveChild(this);
                }

                this._parent = (Element)value;
                if (this._parent != null)
                {
                    this._parent.AddChild(this);
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
            return this._projectEnvoy.GetValidProjectElement() as IProject;
        }

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(this._declaringTypeName.GetNamespaceName());
        }

        public UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = this.GetDeclaredElement();
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

            if (other.GetType() == this.GetType())
            {
                var element = (Element)other;
                string thisFullName;
                string otherFullName;
                try
                {
                    // This might throw for invalid elements.
                    thisFullName = this._declaringTypeName.FullName;
                    otherFullName = element._declaringTypeName.FullName;
                }
                catch (NullReferenceException)
                {
                    return false;
                }

                return other.ShortName == this.ShortName
                       && other.Provider == this.Provider
                       && Equals(element._projectEnvoy, this._projectEnvoy)
                       && thisFullName == otherFullName;
            }
            return false;
        }

        public abstract string GetPresentation();

        public string GetPresentation(IUnitTestElement unitTestElement)
        {
            return this.GetPresentation();
        }

        public abstract IDeclaredElement GetDeclaredElement();

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            ITypeElement declaredType = this.GetDeclaredType();
            if (declaredType == null)
            {
                return EmptyArray<IProjectFile>.Instance;
            }

            return declaredType.GetSourceFiles().Select(x => x.ToProjectFile());
        }

        public IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
        {
            if (this is ContextSpecificationElement)
            {
                var contextSpecification = this as ContextSpecificationElement;
                var context = contextSpecification.Context;

                return new List<UnitTestTask>
               {
                 this._taskFactory.CreateRunAssemblyTask(context),
                 this._taskFactory.CreateContextTask(context),
                 this._taskFactory.CreateContextSpecificationTask(context, contextSpecification)
               };
            }

            if (this is BehaviorSpecificationElement)
            {
                var behaviorSpecification = this as BehaviorSpecificationElement;
                var behavior = behaviorSpecification.Behavior;
                var context = behavior.Context;

                return new List<UnitTestTask>
               {
                 this._taskFactory.CreateRunAssemblyTask(context),
                 this._taskFactory.CreateContextTask(context),
                 this._taskFactory.CreateBehaviorSpecificationTask(context, behaviorSpecification)
               };
            }

            if (this is ContextElement || this is BehaviorElement)
            {
                return EmptyArray<UnitTestTask>.Instance;
            }

            throw new ArgumentException(String.Format("Element is not a Machine.Specifications element: '{0}'", this));
        }

        private static readonly IUnitTestRunStrategy RunStrategy = new OutOfProcessUnitTestRunStrategy(new RemoteTaskRunnerInfo(RecursiveMSpecTaskRunner.RunnerId, typeof(RecursiveMSpecTaskRunner)));

        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return RunStrategy;
        }

        public virtual string GetTitlePrefix()
        {
            return String.Empty;
        }

        protected ITypeElement GetDeclaredType()
        {
            IProject project = this.GetProject();
            if (project == null)
            {
                return null;
            }

            var psiModule = this._psiModuleManager.GetPrimaryPsiModule(project);
            if (psiModule == null)
            {
                return null;
            }

            var declarationsCache = this._cacheManager.GetDeclarationsCache(psiModule, true, true);

            try
            {
                return declarationsCache.GetTypeElementByCLRName(this._declaringTypeName);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public IClrTypeName GetTypeClrName()
        {
            return this._declaringTypeName;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Element;
            if (other == null)
            {
                return false;
            }

            return Equals(other._projectEnvoy, this._projectEnvoy) && other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            var result = 0;
            result = 29 * result + this._projectEnvoy.GetHashCode();
            result = 29 * result + this.Id.GetHashCode();
            return result;
        }

        void AddChild(Element behaviorElement)
        {
            this.Children.Add(behaviorElement);
        }

        void RemoveChild(Element behaviorElement)
        {
            this.Children.Remove(behaviorElement);
        }
    }
}