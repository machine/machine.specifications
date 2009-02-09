using System.Collections.Generic;
using System.Linq;

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

    public FieldElement CreateSpecificationElement(IDeclaredElement field)
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

    public FieldElement CreateSpecificationElement(ContextElement context, IMetadataField specification)
    {
      return new ContextSpecificationElement(_provider,
                                             context,
                                             _project,
                                             specification.DeclaringType.FullyQualifiedName,
                                             specification.Name,
                                             specification.IsIgnored());
    }

    public BehaviorSpecificationElement CreateSpecificationElement(BehaviorElement behavior,
                                                                   IMetadataField specification)
    {
      return new BehaviorSpecificationElement(_provider,
                                              behavior,
                                              _project,
                                              specification.DeclaringType.FullyQualifiedName,
                                              specification.Name,
                                              specification.IsIgnored());
    }

    public IEnumerable<BehaviorSpecificationElement> CreateSpecificationElementsFromBehavior(
      BehaviorElement behaviorElement,
      IMetadataField behavior)
    {
      var behaviorType = ((IMetadataClassType) behavior.Type).Arguments.First();
      var behaviorClass = ((IMetadataClassType) behaviorType).Type;

      foreach (var specification in behaviorClass.GetSpecifications())
      {
        yield return CreateSpecificationElement(behaviorElement, specification);
      }
    }
  }
}