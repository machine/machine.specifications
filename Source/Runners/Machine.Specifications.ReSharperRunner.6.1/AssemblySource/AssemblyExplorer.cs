using System;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.Sdk;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [SolutionComponent]
  public class AssemblyExplorer
  {
    readonly ElementFactories factories;

    public AssemblyExplorer(ElementFactories factories)
    {
      this.factories = factories;
    }

    public void Explore(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer, IMetadataTypeInfo metadataTypeInfo)
    {
        if (!metadataTypeInfo.IsContext())
        {
            return;
        }
        
        //Todo metadataTypeInfo fully replaced with typeInfo
        var contextElement = factories.Contexts.CreateContext(project, assembly.Location.FullPath, metadataTypeInfo);
    
        consumer(contextElement);
        
        //Todo should be typeInfo but GetSpecifications returns ReSharper specific stuff, which is not wanted because typeInfo is a Machine.Specification Interface
     
        metadataTypeInfo.GetSpecifications()
            .ForEach(x => consumer(factories.ContextSpecifications.CreateContextSpecification(contextElement, x)));


        metadataTypeInfo.GetBehaviors().ForEach(x =>
        {
            var behaviorElement = factories.Behaviors.CreateBehavior(contextElement, x);
            consumer(behaviorElement);
            

            factories.BehaviorSpecifications
                        .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
                        .ForEach(y => consumer(y));
        });
    }
  }
}