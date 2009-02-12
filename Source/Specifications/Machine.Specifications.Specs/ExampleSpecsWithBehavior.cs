namespace Machine.Specifications.Specs
{
  [Subject(tag.example)]
  public class context_with_behavior
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<Behavior> behavior;
  }

  [Subject(tag.example)]
  public class context_with_behavior_where_the_behavior_field_is_ignored
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;

    [Ignore]
    Behaves_like<Behavior> behavior;
  }

  [Subject(tag.example)]
  public class context_with_behavior_where_the_behavior_is_ignored
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<IgnoredBehavior> behavior;
  }

  [Subject(tag.example)]
  public class context_with_behavior_where_the_behavior_specs_are_ignored
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorWithIgnoredSpec> behavior;
  }

  [Subject(tag.example)]
  public class context_with_nested_behaviors
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorWithNestedBehavior> behavior_with_nested_behavior;
  }
  
  [Subject(tag.example)]
  public class context_with_behavior_without_behavior_attribute
  {
    public static bool LocalSpecRan;

    It should_run = () => LocalSpecRan = true;
    Behaves_like<BehaviorWithoutBehaviorAttribute> behavior_without_behavior_attribute;
  }
  
  [Behavior]
  public class Behavior
  {
    public static bool BehaviorSpecRan;

    It should_run_if_behavior_is_not_ignored = () => BehaviorSpecRan = true;
  }
  
  [Ignore]
  [Behavior]
  public class IgnoredBehavior
  {
    public static bool BehaviorSpecRan;

    It should_not_run = () => BehaviorSpecRan = true;
  }

  [Behavior]
  public class BehaviorWithIgnoredSpec
  {
    public static bool BehaviorSpecRan;

    [Ignore]
    It should_not_run = () => BehaviorSpecRan = true;
  }

  [Behavior]
  public class BehaviorWithNestedBehavior
  {
    Behaves_like<object> diallowed_nested_behavior;
  }
  
  public class BehaviorWithoutBehaviorAttribute
  {
    public static bool BehaviorSpecRan;

    It should_not_run = () => BehaviorSpecRan = true;
  }
}