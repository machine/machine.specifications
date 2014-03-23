using JetBrains.ProjectModel;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  [SolutionComponent]
  public class ElementFactories
  {
    public ElementFactories(ContextFactory contexts,
                            ContextSpecificationFactory contextSpecifications,
                            BehaviorFactory behaviors,
                            BehaviorSpecificationFactory behaviorSpecifications)
    {
      Contexts = contexts;
      ContextSpecifications = contextSpecifications;
      Behaviors = behaviors;
      BehaviorSpecifications = behaviorSpecifications;
    }

    public ContextFactory Contexts { get; private set; }
    public ContextSpecificationFactory ContextSpecifications { get; private set; }
    public BehaviorFactory Behaviors { get; private set; }
    public BehaviorSpecificationFactory BehaviorSpecifications { get; private set; }
  }
}