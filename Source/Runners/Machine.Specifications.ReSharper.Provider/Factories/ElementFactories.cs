namespace Machine.Specifications.ReSharperProvider.Factories
{
    using JetBrains.ProjectModel;

    [SolutionComponent]
    public class ElementFactories
    {
        public ElementFactories(ContextFactory contexts,
                                ContextSpecificationFactory contextSpecifications,
                                BehaviorFactory behaviors,
                                BehaviorSpecificationFactory behaviorSpecifications)
        {
            this.Contexts = contexts;
            this.ContextSpecifications = contextSpecifications;
            this.Behaviors = behaviors;
            this.BehaviorSpecifications = behaviorSpecifications;
        }

        public ContextFactory Contexts { get; private set; }
        public ContextSpecificationFactory ContextSpecifications { get; private set; }
        public BehaviorFactory Behaviors { get; private set; }
        public BehaviorSpecificationFactory BehaviorSpecifications { get; private set; }
    }
}