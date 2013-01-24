using System.Collections.Generic;

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
  internal class BehaviorSpecificationFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly IProject _project;
#if RESHARPER_61
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();
    readonly IUnitTestElementManager _manager;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;
#endif

#if RESHARPER_61
    public BehaviorSpecificationFactory(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager, IProject project, ProjectModelElementEnvoy projectEnvoy)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public BehaviorSpecificationFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy)
    {
#endif
      _provider = provider;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public IEnumerable<BehaviorSpecificationElement> CreateBehaviorSpecificationsFromBehavior(
      BehaviorElement behavior,
      IMetadataField behaviorSpecification)
    {
      IMetadataTypeInfo typeContainingBehaviorSpecifications = behaviorSpecification.GetFirstGenericArgument();

      foreach (var specification in typeContainingBehaviorSpecifications.GetSpecifications())
      {
        yield return CreateBehaviorSpecification(behavior, specification);
      }
    }

    internal BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                             IDeclaredElement behaviorSpecification)
    {
      return GetOrCreateBehaviorSpecification(_provider,
#if RESHARPER_61
                                              _manager, _psiModuleManager, _cacheManager,
#endif
                                              _project,
                                              behavior,
                                              _projectEnvoy,
                                              ((ITypeMember)behaviorSpecification).GetContainingType().GetClrName().GetPersistent(),
                                              behaviorSpecification.ShortName,
                                              behaviorSpecification.IsIgnored());
    }

    BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                             IMetadataField behaviorSpecification)
    {
      return GetOrCreateBehaviorSpecification(_provider,
#if RESHARPER_61
                                              _manager, _psiModuleManager, _cacheManager,
#endif
                                              _project,
                                              behavior,
                                              _projectEnvoy,
#if RESHARPER_61
                                              _reflectionTypeNameCache.GetClrName(behaviorSpecification.DeclaringType),
#else
                                              new ClrTypeName(behaviorSpecification.DeclaringType.FullyQualifiedName), // may work incorrect in ReSharper 6.0
#endif
                                              behaviorSpecification.Name,
                                              behaviorSpecification.IsIgnored());
    }

    public static BehaviorSpecificationElement GetOrCreateBehaviorSpecification(MSpecUnitTestProvider provider,
#if RESHARPER_61
                                                                                IUnitTestElementManager manager,
                                                                                PsiModuleManager psiModuleManager,
                                                                                CacheManager cacheManager,
#endif
                                                                                IProject project,
                                                                                BehaviorElement behavior,
                                                                                ProjectModelElementEnvoy projectEnvoy,
                                                                                IClrTypeName declaringTypeName,
                                                                                string fieldName,
                                                                                bool isIgnored)
    {
      var id = BehaviorSpecificationElement.CreateId(behavior, fieldName);
#if RESHARPER_61
      var behaviorSpecification = manager.GetElementById(project, id) as BehaviorSpecificationElement;
#else
      var behaviorSpecification = provider.UnitTestManager.GetElementById(project, id) as BehaviorSpecificationElement;
#endif
      if (behaviorSpecification != null)
      {
        behaviorSpecification.Parent = behavior;
        behaviorSpecification.State = UnitTestElementState.Valid;
        return behaviorSpecification;
      }

      return new BehaviorSpecificationElement(provider,
#if RESHARPER_61
                                              psiModuleManager,
                                              cacheManager,
#else
                                              provider.PsiModuleManager,
                                              provider.CacheManager,
#endif
                                              behavior,
                                              projectEnvoy,
                                              declaringTypeName,
                                              fieldName,
                                              isIgnored);
    }
  }
}