using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
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
    readonly ContextCache _cache;
    readonly IProject _project;
#if RESHARPER_61
    readonly IUnitTestElementManager _manager;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;
#endif

#if RESHARPER_61
    public BehaviorFactory(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager, IProject project, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public BehaviorFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
#endif
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public BehaviorElement CreateBehavior(IDeclaredElement field)
    {
      IClass clazz = ((ITypeMember)field).GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context;
      _cache.Classes.TryGetValue(clazz, out context);
      if (context == null)
      {
        return null;
      }

      var fullyQualifiedTypeName = new NormalizedTypeName(field as ITypeOwner);

      return GetOrCreateBehavior(_provider,
#if RESHARPER_61
                                 _manager, _psiModuleManager, _cacheManager,
#endif
                                 _project,
                                 _projectEnvoy,
                                 context,
                                 clazz.GetClrName().FullName,
                                 field.ShortName,
                                 field.IsIgnored(),
                                 fullyQualifiedTypeName);
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
                                                      string declaringTypeName,
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
                                                behavior.DeclaringType.FullyQualifiedName,
                                                behavior.Name,
                                                behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                                fullyQualifiedTypeName);

      return behaviorElement;
    }
  }
}