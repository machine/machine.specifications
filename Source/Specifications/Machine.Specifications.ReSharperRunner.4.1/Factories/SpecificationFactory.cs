using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class SpecificationFactory
  {
    readonly IProjectModelElement _project;
    readonly IUnitTestProvider _provider;

    public SpecificationFactory(IUnitTestProvider provider, IProjectModelElement project)
    {
      _provider = provider;
      _project = project;
    }

    public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
    {
      IClass clazz = field.GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context = ContextCache.Classes[clazz];
      if (context == null)
      {
        return null;
      }

      return new ContextSpecificationElement(_provider,
                                             context,
                                             _project,
                                             clazz.CLRName,
                                             field.ShortName,
                                             field.IsIgnored());
    }

    public ContextSpecificationElement CreateContextSpecification(ContextElement context, IMetadataField specification)
    {
      return new ContextSpecificationElement(_provider,
                                             context,
                                             _project,
                                             specification.DeclaringType.FullyQualifiedName,
                                             specification.Name,
                                             specification.IsIgnored());
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
      BehaviorElement behaviorElement,
      IMetadataField behaviorField)
    {
      IMetadataTypeInfo typeContainingBehaviorSpecifications = behaviorField.GetFirstGenericArgument();

      foreach (var specification in typeContainingBehaviorSpecifications.GetSpecifications())
      {
        yield return CreateBehaviorSpecification(behaviorElement, specification);
      }
    }
  }
}