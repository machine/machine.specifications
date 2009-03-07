namespace Machine.Specifications.Specs
{
  [Subject(tag.example)]
  public class context_with_behaviors
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<Behaviors> behavior;
  }

  [Subject(tag.example)]
  public class context_with_behaviors_where_the_behavior_field_is_ignored
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;

    [Ignore]
    Behaves_like<Behaviors> behavior;
  }

  [Subject(tag.example)]
  public class context_with_behaviors_where_the_behavior_is_ignored
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<IgnoredBehaviors> behavior;
  }

  [Subject(tag.example)]
  public class context_with_behaviors_where_the_behavior_specs_are_ignored
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorsWithIgnoredSpec> behavior;
  }

  [Subject(tag.example)]
  public class context_with_nested_behaviors
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorsWithNestedBehavior> behavior_with_nested_behavior;
  }
  
  [Subject(tag.example)]
  public class context_with_behaviors_without_behaviors_attribute
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorsWithoutBehaviorsAttribute> behavior_without_behavior_attribute;
  }
  
  [Subject(tag.example)]
  public class context_with_behaviors_with_establish
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorsWithEstablish> behavior_with_establish;
  }
  
  [Subject(tag.example)]
  public class context_with_behaviors_with_because
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorsWithBecause> behavior_with_because;
  }
  
  [Behaviors]
  public class Behaviors
  {
    public static bool BehaviorSpecRan;

    It should_run_if_behavior_is_not_ignored = () => BehaviorSpecRan = true;
  }
  
  [Ignore]
  [Behaviors]
  public class IgnoredBehaviors
  {
    public static bool BehaviorSpecRan;

    It should_not_run = () => BehaviorSpecRan = true;
  }

  [Behaviors]
  public class BehaviorsWithIgnoredSpec
  {
    public static bool BehaviorSpecRan;

    [Ignore]
    It should_not_run = () => BehaviorSpecRan = true;
  }

  [Behaviors]
  public class BehaviorsWithNestedBehavior
  {
    Behaves_like<object> diallowed_nested_behavior;
  }
  
  public class BehaviorsWithoutBehaviorsAttribute
  {
    public static bool BehaviorSpecRan;

    It should_not_run = () => BehaviorSpecRan = true;
  }

  [Behaviors]
  public class BehaviorsWithEstablish
  {
    Establish context;
  }
  
  [Behaviors]
  public class BehaviorsWithBecause
  {
    Because of;
  }
}