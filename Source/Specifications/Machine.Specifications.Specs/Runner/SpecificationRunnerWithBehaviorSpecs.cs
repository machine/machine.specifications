using System;

namespace Machine.Specifications.Specs.Runner
{
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behavior.LocalSpecRan = false;
        Behavior.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behavior>;

    It should_run_the_context_spec = () => context_with_behavior.LocalSpecRan.ShouldBeTrue();
    It should_run_the_behavior_spec = () => Behavior.BehaviorSpecRan.ShouldBeTrue();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_field_is_ignored
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behavior_where_the_behavior_field_is_ignored.LocalSpecRan = false;
        Behavior.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behavior_where_the_behavior_field_is_ignored>;

    It should_run_the_context_spec = () => context_with_behavior_where_the_behavior_field_is_ignored.LocalSpecRan.ShouldBeTrue();
    It should_not_run_the_behavior_spec = () => Behavior.BehaviorSpecRan.ShouldBeFalse();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_is_ignored
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behavior_where_the_behavior_is_ignored.LocalSpecRan = false;
        IgnoredBehavior.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behavior_where_the_behavior_is_ignored>;

    It should_run_the_context_spec = () => context_with_behavior_where_the_behavior_is_ignored.LocalSpecRan.ShouldBeTrue();
    It should_not_run_the_behavior_spec = () => IgnoredBehavior.BehaviorSpecRan.ShouldBeFalse();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_specs_are_ignored
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behavior_where_the_behavior_specs_are_ignored.LocalSpecRan = false;
        BehaviorWithIgnoredSpec.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behavior_where_the_behavior_specs_are_ignored>;

    It should_run_the_context_spec = () => context_with_behavior_where_the_behavior_specs_are_ignored.LocalSpecRan.ShouldBeTrue();
    It should_not_run_the_behavior_spec = () => BehaviorWithIgnoredSpec.BehaviorSpecRan.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_nested_behaviors
    : with_runner
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_nested_behaviors>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_behaviors_that_do_not_have_the_behavior_attribute
    : with_runner
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_behavior_without_behavior_attribute>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_behaviors_with_establish
    : with_runner
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_behavior_with_establish>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
  }
}