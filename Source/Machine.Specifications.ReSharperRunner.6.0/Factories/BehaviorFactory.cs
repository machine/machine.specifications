using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class BehaviorFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly ElementCache _cache;
    readonly IProject _project;
#if RESHARPER_61
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();
    readonly IUnitTestElementManager _manager;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;
#endif

#if RESHARPER_61
    public BehaviorFactory(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager, IProject project, ProjectModelElementEnvoy projectEnvoy, ElementCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public BehaviorFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, ElementCache cache)
    {
#endif
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public BehaviorElement CreateBehavior(IDeclaredElement field)
    {
      var clazz = ((ITypeMember)field).GetContainingType() as IClass;
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
#if RESHARPER_61
                                 _manager, _psiModuleManager, _cacheManager,
#endif
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
#if RESHARPER_61
                                                _manager, _psiModuleManager, _cacheManager,
#endif
                                                _project,
                                                _projectEnvoy,
                                                context,
#if RESHARPER_61
                                                _reflectionTypeNameCache.GetClrName(behavior.DeclaringType),
#else
                                                new ClrTypeName(behavior.DeclaringType.FullyQualifiedName), // may work incorrect in ReSharper 6.0
#endif

                                                behavior.Name,
                                                behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                                fullyQualifiedTypeName);

      return behaviorElement;
    }

    public static BehaviorElement GetOrCreateBehavior(MSpecUnitTestProvider provider,
#if RESHARPER_61
                                                      IUnitTestElementManager manager,
                                                      PsiModuleManager psiModuleManager,
                                                      CacheManager cacheManager,
#endif
                                                      IProject project,
                                                      ProjectModelElementEnvoy projectEnvoy,
                                                      ContextElement context,
                                                      IClrTypeName declaringTypeName,
                                                      string fieldName,
                                                      bool isIgnored,
                                                      string fullyQualifiedTypeName)
    {
      var id = BehaviorElement.CreateId(context, fullyQualifiedTypeName, fieldName);
#if RESHARPER_61
      var behavior = manager.GetElementById(project, id) as BehaviorElement;
#else
      var behavior = provider.UnitTestManager.GetElementById(project, id) as BehaviorElement;
#endif
      if (behavior != null)
      {
        behavior.Parent = context;
        behavior.State = UnitTestElementState.Valid;
        return behavior;
      }

      return new BehaviorElement(provider,
#if RESHARPER_61
                                 psiModuleManager, cacheManager,
#else
                                 provider.PsiModuleManager, provider.CacheManager,
#endif
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
