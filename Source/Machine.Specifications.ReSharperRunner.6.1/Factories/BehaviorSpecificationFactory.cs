using System.Collections.Generic;

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
  class BehaviorSpecificationFactory
  {
    readonly CacheManager _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly IProject _project;
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly PsiModuleManager _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public BehaviorSpecificationFactory(MSpecUnitTestProvider provider,
                                        IUnitTestElementManager manager,
                                        PsiModuleManager psiModuleManager,
                                        CacheManager cacheManager,
                                        IProject project,
                                        ProjectModelElementEnvoy projectEnvoy)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public IEnumerable<BehaviorSpecificationElement> CreateBehaviorSpecificationsFromBehavior(
      BehaviorElement behavior,
      IMetadataField behaviorSpecification)
    {
      var typeContainingBehaviorSpecifications = behaviorSpecification.GetFirstGenericArgument();

      foreach (var specification in typeContainingBehaviorSpecifications.GetSpecifications())
      {
        yield return CreateBehaviorSpecification(behavior, specification);
      }
    }

    internal BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                                      IDeclaredElement behaviorSpecification)
    {
      return GetOrCreateBehaviorSpecification(_provider,
                                              _manager,
                                              _psiModuleManager,
                                              _cacheManager,
                                              _project,
                                              behavior,
                                              _projectEnvoy,
                                              ((ITypeMember) behaviorSpecification).GetContainingType()
                                                                                   .GetClrName()
                                                                                   .GetPersistent(),
                                              behaviorSpecification.ShortName,
                                              behaviorSpecification.IsIgnored());
    }

    BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                             IMetadataField behaviorSpecification)
    {
      return GetOrCreateBehaviorSpecification(_provider,
                                              _manager,
                                              _psiModuleManager,
                                              _cacheManager,
                                              _project,
                                              behavior,
                                              _projectEnvoy,
                                              _reflectionTypeNameCache.GetClrName(behaviorSpecification.DeclaringType),
                                              behaviorSpecification.Name,
                                              behaviorSpecification.IsIgnored());
    }

    public static BehaviorSpecificationElement GetOrCreateBehaviorSpecification(MSpecUnitTestProvider provider,
                                                                                IUnitTestElementManager manager,
                                                                                PsiModuleManager psiModuleManager,
                                                                                CacheManager cacheManager,
                                                                                IProject project,
                                                                                BehaviorElement behavior,
                                                                                ProjectModelElementEnvoy projectEnvoy,
                                                                                IClrTypeName declaringTypeName,
                                                                                string fieldName,
                                                                                bool isIgnored)
    {
      var id = BehaviorSpecificationElement.CreateId(behavior, fieldName);
      var behaviorSpecification = manager.GetElementById(project, id) as BehaviorSpecificationElement;
      if (behaviorSpecification != null)
      {
        behaviorSpecification.Parent = behavior;
        behaviorSpecification.State = UnitTestElementState.Valid;
        return behaviorSpecification;
      }

      return new BehaviorSpecificationElement(provider,
                                              psiModuleManager,
                                              cacheManager,
                                              behavior,
                                              projectEnvoy,
                                              declaringTypeName,
                                              fieldName,
                                              isIgnored);
    }
  }
}