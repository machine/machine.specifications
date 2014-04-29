namespace Machine.Specifications.ReSharperProvider.Factories
{
    using System.Linq;

    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Impl.Reflection2;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Elements;

    using Machine.Specifications.ReSharperProvider.Presentation;
    using Machine.Specifications.ReSharperProvider.Shims;

    [SolutionComponent]
    public class BehaviorFactory
    {
        readonly ElementCache _cache;
        readonly IUnitTestElementManager _manager;
        readonly ICache _cacheManager;
        readonly MSpecUnitTestProvider _provider;
        readonly IPsi _psiModuleManager;
        readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

        public BehaviorFactory(MSpecUnitTestProvider provider,
                             IUnitTestElementManager manager,
                             IPsi psiModuleManager,
                             ICache cacheManager,
                             ElementCache cache)
        {
            this._psiModuleManager = psiModuleManager;
            this._cacheManager = cacheManager;
            this._provider = provider;
            this._cache = cache;
            this._manager = manager;
        }

        public BehaviorElement CreateBehavior(IDeclaredElement field)
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

            var fieldType = new NormalizedTypeName(field as ITypeOwner);

            var behavior = this.GetOrCreateBehavior(context,
                                               clazz.GetClrName(),
                                               field.ShortName,
                                               field.IsIgnored(),
                                               fieldType);

            foreach (var child in behavior.Children)
            {
                child.State = UnitTestElementState.Pending;
            }

            this._cache.AddBehavior(field, behavior);
            return behavior;
        }

        public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior)
        {
            var typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

            var metadataTypeName = behavior.FirstGenericArgumentClass().FullyQualifiedName();
            var fieldType = new NormalizedTypeName(new ClrTypeName(metadataTypeName));

            var behaviorElement = this.GetOrCreateBehavior(context,
                                                      this._reflectionTypeNameCache.GetClrName(behavior.DeclaringType),
                                                      behavior.Name,
                                                      behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                                      fieldType);

            return behaviorElement;
        }

        public BehaviorElement GetOrCreateBehavior(ContextElement context,
                                                   IClrTypeName declaringTypeName,
                                                   string fieldName,
                                                   bool isIgnored,
                                                   string fieldType)
        {
            var id = BehaviorElement.CreateId(context, fieldType, fieldName);
            var behavior = this._manager.GetElementById(context.GetProject(), id) as BehaviorElement;
            if (behavior != null)
            {
                behavior.Parent = context;
                behavior.State = UnitTestElementState.Valid;
                return behavior;
            }

            return new BehaviorElement(this._provider,
                                       this._psiModuleManager,
                                       this._cacheManager,
                                       context,
                                       new ProjectModelElementEnvoy(context.GetProject()),
                                       declaringTypeName,
                                       fieldName,
                                       isIgnored,
                                       fieldType);
        }

        public void UpdateChildState(IDeclaredElement field)
        {
            var behavior = this._cache.TryGetBehavior(field);
            if (behavior == null)
            {
                return;
            }

            foreach (var element in behavior
              .Children.Where(x => x.State == UnitTestElementState.Pending)
              .Flatten(x => x.Children))
            {
                element.State = UnitTestElementState.Invalid;
            }
        }
    }
}