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

    public void Explore(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer, IMetadataTypeInfo metadataTypeInfo)
    {
      if (!metadataTypeInfo.IsContext())
      {
        return;
      }

      var contextElement = _factories.Contexts.CreateContext(project, assembly.Location.FullPath, metadataTypeInfo);

      consumer(contextElement);

      metadataTypeInfo.GetSpecifications()
          .ForEach(x => consumer(_factories.ContextSpecifications.CreateContextSpecification(contextElement, x)));


      metadataTypeInfo.GetBehaviors().ForEach(x =>
      {
        var behaviorElement = _factories.Behaviors.CreateBehavior(contextElement, x);
        consumer(behaviorElement);


        _factories.BehaviorSpecifications
                    .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
                    .ForEach(y => consumer(y));
      });
    }
  }
}