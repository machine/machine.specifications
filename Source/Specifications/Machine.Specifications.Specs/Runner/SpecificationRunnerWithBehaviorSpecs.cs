using System;

namespace Machine.Specifications.Specs.Runner
{
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behaviors.LocalSpecRan = false;
        Behaviors.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behaviors>;

    It should_run_the_context_spec = () => context_with_behaviors.LocalSpecRan.ShouldBeTrue();
    It should_run_the_behavior_spec = () => Behaviors.BehaviorSpecRan.ShouldBeTrue();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_field_is_ignored
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behaviors_where_the_behavior_field_is_ignored.LocalSpecRan = false;
        Behaviors.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behaviors_where_the_behavior_field_is_ignored>;

    It should_run_the_context_spec = () => context_with_behaviors_where_the_behavior_field_is_ignored.LocalSpecRan.ShouldBeTrue();
    It should_not_run_the_behavior_spec = () => Behaviors.BehaviorSpecRan.ShouldBeFalse();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_is_ignored
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behaviors_where_the_behavior_is_ignored.LocalSpecRan = false;
        IgnoredBehaviors.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behaviors_where_the_behavior_is_ignored>;

    It should_run_the_context_spec = () => context_with_behaviors_where_the_behavior_is_ignored.LocalSpecRan.ShouldBeTrue();
    It should_not_run_the_behavior_spec = () => IgnoredBehaviors.BehaviorSpecRan.ShouldBeFalse();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_specs_are_ignored
    : with_runner
  {
    Establish context = () =>
      {
        context_with_behaviors_where_the_behavior_specs_are_ignored.LocalSpecRan = false;
        BehaviorsWithIgnoredSpec.BehaviorSpecRan = false;
      };

    Because of = Run<context_with_behaviors_where_the_behavior_specs_are_ignored>;

    It should_run_the_context_spec = () => context_with_behaviors_where_the_behavior_specs_are_ignored.LocalSpecRan.ShouldBeTrue();
    It should_not_run_the_behavior_spec = () => BehaviorsWithIgnoredSpec.BehaviorSpecRan.ShouldBeFalse();
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

    Because of = () => { Exception = Catch.Exception(Run<context_with_behaviors_without_behaviors_attribute>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_behaviors_with_establish
    : with_runner
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_behaviors_with_establish>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_behaviors_with_because
    : with_runner
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_behaviors_with_because>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
  }
}