namespace Machine.Specifications.ReSharperProvider.Factories
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Impl.Reflection2;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Elements;

    using Machine.Specifications.ReSharperProvider.Presentation;
    using Machine.Specifications.ReSharperProvider.Shims;

    [SolutionComponent]
    public class ContextSpecificationFactory
    {
        readonly ElementCache _cache;
        readonly ICache _cacheManager;
        readonly IUnitTestElementManager _manager;
        readonly MSpecUnitTestProvider _provider;
        readonly IPsi _psiModuleManager;
        readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

        public ContextSpecificationFactory(MSpecUnitTestProvider provider,
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

        public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
        {
            var clazz = ((ITypeMember)field).GetContainingType() as IClass;
            if (clazz == null)
            {
                return null;
            }

            var context = this._cache.TryGetContext(clazz);
            if (context == null)
            {
                return null;
            }

            return this.GetOrCreateContextSpecification(context,
                                                   clazz.GetClrName().GetPersistent(),
                                                   field.ShortName,
                                                   field.IsIgnored());
        }

        public ContextSpecificationElement CreateContextSpecification(ContextElement context, IMetadataField specification)
        {
            return this.GetOrCreateContextSpecification(context,
                                                   this._reflectionTypeNameCache.GetClrName(specification.DeclaringType),
                                                   specification.Name,
                                                   specification.IsIgnored());
        }

        public ContextSpecificationElement GetOrCreateContextSpecification(ContextElement context,
                                                                           IClrTypeName declaringTypeName,
                                                                           string fieldName,
                                                                           bool isIgnored)
        {
            var id = ContextSpecificationElement.CreateId(context, fieldName);
            var contextSpecification = this._manager.GetElementById(context.GetProject(), id) as ContextSpecificationElement;
            if (contextSpecification != null)
            {
                contextSpecification.Parent = context;
                contextSpecification.State = UnitTestElementState.Valid;
                return contextSpecification;
            }

            return new ContextSpecificationElement(this._provider,
                                                   this._psiModuleManager,
                                                   this._cacheManager,
                                                   new ProjectModelElementEnvoy(context.GetProject()),
                                                   context,
                                                   declaringTypeName,
                                                   fieldName, isIgnored);
        }
    }
}