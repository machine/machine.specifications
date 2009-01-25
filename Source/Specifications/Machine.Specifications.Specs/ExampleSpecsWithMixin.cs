namespace Machine.Specifications.Specs
{
  public class context_with_behavior
  {
    public static bool LocalSpecRan;

    It should_succeed = () => LocalSpecRan = true;
    It_should_behave_like behavior = () => new Behavior();
  }
  
  public class context_with_behavior_where_the_behavior_field_is_ignored
  {
    public static bool LocalSpecRan;

    It should_succeed = () => LocalSpecRan = true;
    
    [Ignore]
    It_should_behave_like behavior = () => new Behavior();
  }
  
  public class context_with_behavior_where_the_behavior_is_ignored
  {
    public static bool LocalSpecRan;

    It should_succeed = () => LocalSpecRan = true;
    
    It_should_behave_like behavior = () => new IgnoredBehavior();
  }
  
  public class context_with_behavior_where_the_behavior_specs_are_ignored
  {
    public static bool LocalSpecRan;

    It should_succeed = () => LocalSpecRan = true;

    It_should_behave_like behavior = () => new BehaviorWithIgnoredSpec();
  }

  public class context_with_nested_behaviors
  {
    public static bool LocalSpecRan;

    It should_succeed = () => LocalSpecRan = true;
    It_should_behave_like behavior_with_nested_behavior = () => new BehaviorWithNestedBehavior();
  }

  internal class Behavior
  {
    public static bool BehaviorSpecRan;

    It should_succeed_on_the_behavior = () => BehaviorSpecRan = true;
  }
  
  [Ignore]
  internal class IgnoredBehavior
  {
    public static bool BehaviorSpecRan;

    It should_succeed_on_the_behavior = () => BehaviorSpecRan = true;
  }
  
  internal class BehaviorWithIgnoredSpec
  {
    public static bool BehaviorSpecRan;
    
    [Ignore]
    It should_succeed_on_the_behavior = () => BehaviorSpecRan = true;
  }

  internal class BehaviorWithNestedBehavior
  {
    It_should_behave_like nested_behavior;
  }
}