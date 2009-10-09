using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class BehaviorSpecificationFactory
  {
    readonly IProjectModelElement _project;
    readonly IUnitTestProvider _provider;

    public BehaviorSpecificationFactory(IUnitTestProvider provider, IProjectModelElement project)
    {
      _provider = provider;
      _project = project;
    }

    BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                             IMetadataField behaviorSpecification)
    {
      return new BehaviorSpecificationElement(_provider,
                                              behavior,
                                              _project,
                                              behaviorSpecification.DeclaringType.FullyQualifiedName,
                                              behaviorSpecification.Name,
                                              behaviorSpecification.IsIgnored());
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

    public IEnumerable<BehaviorSpecificationElement> CreateBehaviorSpecificationsFromBehavior(BehaviorElement behavior,
                                                                                              IDeclaredElement
                                                                                                behaviorSpecification)
    {
      IClass typeContainingBehaviorSpecifications = behaviorSpecification.GetFirstGenericArgument();

      foreach (IField specification in typeContainingBehaviorSpecifications.GetBehaviorSpecifications())
      {
        yield return CreateBehaviorSpecification(behavior, specification);
      }
    }

    BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                             IDeclaredElement behaviorSpecification)
    {
      return new BehaviorSpecificationElement(_provider,
                                              behavior,
                                              _project,
                                              behaviorSpecification.GetContainingType().CLRName,
                                              behaviorSpecification.ShortName,
                                              behaviorSpecification.IsIgnored());
    }
  }
}