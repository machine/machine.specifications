using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;

    using Machine.Specifications.ReSharperProvider.Factories;

    [SolutionComponent]
    public class AssemblyExplorer
    {
        readonly ElementFactories _factories;

        public AssemblyExplorer(ElementFactories factories)
        {
            this._factories = factories;
        }

        public void Explore(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer, IMetadataTypeInfo metadataTypeInfo)
        {
            if (!metadataTypeInfo.IsContext())
            {
                return;
            }

            var contextElement = this._factories.Contexts.CreateContext(project, assembly.Location.FullPath, metadataTypeInfo);

            consumer(contextElement);

            metadataTypeInfo.GetSpecifications()
                .ForEach(x => consumer(this._factories.ContextSpecifications.CreateContextSpecification(contextElement, x)));


            metadataTypeInfo.GetBehaviors().ForEach(x =>
            {
                var behaviorElement = this._factories.Behaviors.CreateBehavior(contextElement, x);
                consumer(behaviorElement);


                this._factories.BehaviorSpecifications
                            .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
                            .ForEach(y => consumer(y));
            });
        }
    }
}