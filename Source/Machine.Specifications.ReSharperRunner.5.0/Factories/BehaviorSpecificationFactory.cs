using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class BehaviorSpecificationFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly IProject _project;

    public BehaviorSpecificationFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy)
    {
      _provider = provider;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                             IMetadataField behaviorSpecification)
    {
      return GetOrCreateBehaviorSpecification(_provider,
                                              _project,
                                              behavior,
                                              _projectEnvoy,
                                              behavior.FullyQualifiedTypeName ?? behaviorSpecification.DeclaringType.FullyQualifiedName,
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
                                                                                              IDeclaredElement behaviorSpecification)
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
      return GetOrCreateBehaviorSpecification(_provider,
                                              _project,
                                              behavior,
                                              _projectEnvoy,
#if RESHARPER_6
                                              behavior.FullyQualifiedTypeName ?? ((ITypeMember)behaviorSpecification).GetContainingType().GetClrName().FullName,
#else
                                              behavior.FullyQualifiedTypeName ?? behaviorSpecification.GetContainingType().CLRName,
#endif
 behaviorSpecification.ShortName,
                                              behaviorSpecification.IsIgnored());
    }

    public static BehaviorSpecificationElement GetOrCreateBehaviorSpecification(MSpecUnitTestProvider provider, IProject project, BehaviorElement behavior, ProjectModelElementEnvoy projectEnvoy, string declaringTypeName, string fieldName, bool isIgnored)
    {
#if RESHARPER_6
      var id = BehaviorSpecificationElement.CreateId(behavior, fieldName);
      var behaviorSpecification = provider.UnitTestManager.GetElementById(project, id) as BehaviorSpecificationElement;
      if (behaviorSpecification != null)
      {
        behaviorSpecification.Parent = behavior;
        behaviorSpecification.State = UnitTestElementState.Valid;
        return behaviorSpecification;
      }
#endif

      return new BehaviorSpecificationElement(provider,
                                              behavior,
                                              projectEnvoy,
                                              declaringTypeName,
                                              fieldName,
                                              isIgnored);
    }
  }
}