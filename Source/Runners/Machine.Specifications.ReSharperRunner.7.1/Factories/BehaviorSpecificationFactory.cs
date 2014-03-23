using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Shims;

using ICache = Machine.Specifications.ReSharperRunner.Shims.ICache;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  [SolutionComponent]
  public class BehaviorSpecificationFactory
  {
    readonly ICache _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly MSpecUnitTestProvider _provider;
    readonly IPsi _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public BehaviorSpecificationFactory(MSpecUnitTestProvider provider,
                                        IUnitTestElementManager manager,
                                        IPsi psiModuleManager,
                                        ICache cacheManager)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
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
      return GetOrCreateBehaviorSpecification(behavior,
                                              ((ITypeMember) behaviorSpecification).GetContainingType()
                                                                                   .GetClrName()
                                                                                   .GetPersistent(),
                                              behaviorSpecification.ShortName,
                                              behaviorSpecification.IsIgnored());
    }

    BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                             IMetadataField behaviorSpecification)
    {
      return GetOrCreateBehaviorSpecification(behavior,
                                              _reflectionTypeNameCache.GetClrName(behaviorSpecification.DeclaringType),
                                              behaviorSpecification.Name,
                                              behaviorSpecification.IsIgnored());
    }

    public BehaviorSpecificationElement GetOrCreateBehaviorSpecification(BehaviorElement behavior,
                                                                         IClrTypeName declaringTypeName,
                                                                         string fieldName,
                                                                         bool isIgnored)
    {
      var id = BehaviorSpecificationElement.CreateId(behavior, fieldName);
      var behaviorSpecification = _manager.GetElementById(behavior.GetProject(), id) as BehaviorSpecificationElement;
      if (behaviorSpecification != null)
      {
        behaviorSpecification.Parent = behavior;
        behaviorSpecification.State = UnitTestElementState.Valid;
        return behaviorSpecification;
      }

      return new BehaviorSpecificationElement(_provider,
                                              _psiModuleManager,
                                              _cacheManager,
                                              new ProjectModelElementEnvoy(behavior.GetProject()),
                                              behavior,
                                              declaringTypeName,
                                              fieldName,
                                              isIgnored);
    }
  }
}