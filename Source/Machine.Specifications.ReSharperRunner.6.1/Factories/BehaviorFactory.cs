using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  class BehaviorFactory
  {
    readonly ElementCache _cache;
    readonly CacheManager _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly IProject _project;
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly PsiModuleManager _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public BehaviorFactory(MSpecUnitTestProvider provider,
                           IUnitTestElementManager manager,
                           PsiModuleManager psiModuleManager,
                           CacheManager cacheManager,
                           IProject project,
                           ProjectModelElementEnvoy projectEnvoy,
                           ElementCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public BehaviorElement CreateBehavior(IDeclaredElement field)
    {
      var clazz = ((ITypeMember) field).GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context;
      _cache.Contexts.TryGetValue(clazz, out context);
      if (context == null)
      {
        return null;
      }

      var fullyQualifiedTypeName = new NormalizedTypeName(field as ITypeOwner);

      var behavior = GetOrCreateBehavior(_provider,
                                         _manager,
                                         _psiModuleManager,
                                         _cacheManager,
                                         _project,
                                         _projectEnvoy,
                                         context,
                                         clazz.GetClrName(),
                                         field.ShortName,
                                         field.IsIgnored(),
                                         fullyQualifiedTypeName);

      foreach (var child in behavior.Children)
      {
        child.State = UnitTestElementState.Pending;
      }

      _cache.Behaviors.Add(field, behavior);
      return behavior;
    }

    public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior)
    {
      var typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

      var metadataTypeName = behavior.FirstGenericArgumentClass().FullyQualifiedName();
      var fullyQualifiedTypeName = new NormalizedTypeName(metadataTypeName);

      var behaviorElement = GetOrCreateBehavior(_provider,
                                                _manager,
                                                _psiModuleManager,
                                                _cacheManager,
                                                _project,
                                                _projectEnvoy,
                                                context,
                                                _reflectionTypeNameCache.GetClrName(behavior.DeclaringType),
                                                behavior.Name,
                                                behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                                fullyQualifiedTypeName);

      return behaviorElement;
    }

    public static BehaviorElement GetOrCreateBehavior(MSpecUnitTestProvider provider,
                                                      IUnitTestElementManager manager,
                                                      PsiModuleManager psiModuleManager,
                                                      CacheManager cacheManager,
                                                      IProject project,
                                                      ProjectModelElementEnvoy projectEnvoy,
                                                      ContextElement context,
                                                      IClrTypeName declaringTypeName,
                                                      string fieldName,
                                                      bool isIgnored,
                                                      string fullyQualifiedTypeName)
    {
      var id = BehaviorElement.CreateId(context, fullyQualifiedTypeName, fieldName);
      var behavior = manager.GetElementById(project, id) as BehaviorElement;
      if (behavior != null)
      {
        behavior.Parent = context;
        behavior.State = UnitTestElementState.Valid;
        return behavior;
      }

      return new BehaviorElement(provider,
                                 psiModuleManager,
                                 cacheManager,
                                 context,
                                 projectEnvoy,
                                 declaringTypeName,
                                 fieldName,
                                 isIgnored,
                                 fullyQualifiedTypeName);
    }

    public void UpdateChildState(IDeclaredElement field)
    {
      BehaviorElement behavior;
      if (!_cache.Behaviors.TryGetValue(field, out behavior))
      {
        return;
      }

      foreach (var element in behavior
        .Children.Where(x => x.State == UnitTestElementState.Pending)
        .Traverse(x => x.Children))
      {
        element.State = UnitTestElementState.Invalid;
      }
    }
  }
}