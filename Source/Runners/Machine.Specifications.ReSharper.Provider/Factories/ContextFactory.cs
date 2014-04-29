namespace Machine.Specifications.ReSharperProvider.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Impl.Reflection2;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Elements;

    using Machine.Specifications.ReSharperProvider.Presentation;
    using Machine.Specifications.ReSharperProvider.Shims;

    [SolutionComponent]
    public class ContextFactory
    {
        readonly ElementCache _cache;
        readonly ICache _cacheManager;
        readonly IUnitTestElementManager _manager;
        readonly MSpecUnitTestProvider _provider;
        readonly IPsi _psiModuleManager;
        readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

        public ContextFactory(MSpecUnitTestProvider provider,
                              IUnitTestElementManager manager,
                              IPsi psiModuleManager,
                              ICache cacheManager,
                              ElementCache cache)
        {
            this._manager = manager;
            this._psiModuleManager = psiModuleManager;
            this._cacheManager = cacheManager;
            this._provider = provider;
            this._cache = cache;
        }

        public IUnitTestElement CreateContext(string assemblyPath, IDeclaration declaration)
        {
            var type = (ITypeElement)declaration.DeclaredElement;
            var context = this.GetOrCreateContext(assemblyPath,
                                             declaration.GetProject(),
                                             type.GetClrName().GetPersistent(),
                                             type.GetSubjectString(),
                                             type.GetTags(), type.IsIgnored());

            foreach (var child in context.Children)
            {
                child.State = UnitTestElementState.Pending;
            }

            this._cache.AddContext(type, context);
            return context;
        }

        public ContextElement CreateContext(IProject project, string assemblyPath, IMetadataTypeInfo type)
        {
            return this.GetOrCreateContext(assemblyPath,
                                      project,
                                      this._reflectionTypeNameCache.GetClrName(type),
                                      type.GetSubjectString(),
                                      type.GetTags(), type.IsIgnored());
        }

        public ContextElement GetOrCreateContext(string assemblyPath,
                                                 IProject project,
                                                 IClrTypeName typeName,
                                                 string subject,
                                                 ICollection<string> tags,
                                                 bool isIgnored)
        {
            var id = ContextElement.CreateId(subject, typeName.FullName, tags);
            var contextElement = this._manager.GetElementById(project, id) as ContextElement;
            if (contextElement != null)
            {
                contextElement.State = UnitTestElementState.Valid;
                return contextElement;
            }

            return new ContextElement(this._provider,
                                      this._psiModuleManager,
                                      this._cacheManager,
                                      new ProjectModelElementEnvoy(project),
                                      typeName,
                                      assemblyPath,
                                      subject,
                                      tags,
                                      isIgnored);
        }

        public void UpdateChildState(ITypeElement type)
        {
            var context = this._cache.TryGetContext(type);
            if (context == null)
            {
                return;
            }

            foreach (var element in context
              .Children.Where(x => x.State == UnitTestElementState.Pending)
              .Flatten(x => x.Children))
            {
                element.State = UnitTestElementState.Invalid;
            }
        }
    }
}