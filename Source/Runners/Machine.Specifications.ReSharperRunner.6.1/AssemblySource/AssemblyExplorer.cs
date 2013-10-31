using System;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
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
        ITypeInfo typeInfo = metadataTypeInfo.AsTypeInfo();
        if (!SpecTypeCheck.IsSpecClass(typeInfo))
        {
            return;
        }
        
        //Todo metadataTypeInfo fully replaced with typeInfo
        var contextElement = factories.Contexts.CreateContext(project, assembly.Location.FullPath, metadataTypeInfo, typeInfo);
    
        consumer(contextElement);
        
        //Todo should be typeInfo but GetSpecifications returns ReSharper specific stuff, which is not wanted because typeInfo is a Machine.Specification Interface
        foreach (var r in metadataTypeInfo.GetSpecifications())
        {
             string e = r.Name;
        }

        metadataTypeInfo.GetSpecifications()
            .ForEach(x => consumer(factories.ContextSpecifications.CreateContextSpecification(contextElement, x)));


        foreach (var b in metadataTypeInfo.GetBehaviors())
        {
            string c = b.Name;
        }

        metadataTypeInfo.GetBehaviors().ForEach(x =>
        {
            var behaviorElement = factories.Behaviors.CreateBehavior(contextElement, x);
            consumer(behaviorElement);
            

            factories.BehaviorSpecifications
                        .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
                        .ForEach(y => consumer(y));
        });
    }

    //public void Explore(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
    //{

    //  //Checks if there exists a referenced assembly to the Machine.Specification, if not it cannot be a test which contains specs
    //  //if (!assembly.ReferencedAssembliesNames.Any(x => String.Equals(
    //  //                                                               x.Name,
    //  //                                                               typeof(It).Assembly.GetName().Name,
    //  //                                                               StringComparison.InvariantCultureIgnoreCase)))
    //  //{
    //  //  return;
    //  //}


    //  assembly.GetTypes().Where(type => type.IsContext()).ForEach(type =>
    //  {
    //    var contextElement = factories.Contexts.CreateContext(project,assembly.Location.FullPath, type);
    //    consumer(contextElement);

    //    type
    //      .GetSpecifications()
    //      .ForEach(x => consumer(factories.ContextSpecifications.CreateContextSpecification(contextElement, x)));

    //    type.GetBehaviors().ForEach(x =>
    //    {
    //      var behaviorElement = factories.Behaviors.CreateBehavior(contextElement, x);
    //      consumer(behaviorElement);

    //      factories.BehaviorSpecifications
    //                .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
    //                .ForEach(y => consumer(y));
    //    });
    //  });
    //}

 
  }
}