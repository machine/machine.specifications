using System;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [SolutionComponent]
  public class AssemblyExplorer
  {
    readonly ElementFactories _factories;

    public AssemblyExplorer(ElementFactories factories)
    {
      _factories = factories;
    }

    public void Explore(IMetadataAssembly assembly, UnitTestElementConsumer consumer)
    {
      if (!assembly.ReferencedAssembliesNames.Any(x => String.Equals(
                                                                     x.Name,
                                                                     typeof(It).Assembly.GetName().Name,
                                                                     StringComparison.InvariantCultureIgnoreCase)))
      {
        return;
      }

      assembly.GetTypes().Where(type => type.IsContext()).ForEach(type =>
      {
        var contextElement = _factories.Contexts.CreateContext(type);
        consumer(contextElement);

        type
          .GetSpecifications()
          .ForEach(x => consumer(_factories.ContextSpecifications.CreateContextSpecification(contextElement, x)));

        type.GetBehaviors().ForEach(x =>
        {
          var behaviorElement = _factories.Behaviors.CreateBehavior(contextElement, x);
          consumer(behaviorElement);

          _factories.BehaviorSpecifications
                    .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
                    .ForEach(y => consumer(y));
        });
      });
    }
  }
}