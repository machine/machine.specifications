using System;

namespace Machine.Specifications.Specs.Runner
{
  [Subject("Specification Runner")]
  public class when_running_a_context_with_specifications_in_a_behavior
    : RunnerSpecs
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
    : RunnerSpecs
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
    : RunnerSpecs
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
    : RunnerSpecs
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
    : RunnerSpecs
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_nested_behaviors>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
    It should_print_the_type_containing_the_nested_behaviors = () => 
      Exception.Message.ShouldContain(typeof(BehaviorsWithNestedBehavior).FullName);
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_behaviors_that_do_not_have_the_behaviors_attribute
    : RunnerSpecs
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_behaviors_without_behaviors_attribute>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
    It should_print_the_type_missing_the_attribute = () =>
      Exception.Message.ShouldContain(typeof(BehaviorsWithoutBehaviorsAttribute).FullName);
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_behaviors_with_establish
    : RunnerSpecs
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_behaviors_with_establish>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
    It should_print_the_behaviors_with_the_establish = () =>
      Exception.Message.ShouldContain(typeof(BehaviorsWithEstablish).FullName);
  }
  
  [Subject("Specification Runner")]
  public class when_running_a_context_with_behaviors_with_because
    : RunnerSpecs
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_behaviors_with_because>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
    It should_print_the_behaviors_with_the_because = () =>
      Exception.Message.ShouldContain(typeof(BehaviorsWithBecause).FullName);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_that_does_not_have_all_fields_needed_by_the_behavior
    : RunnerSpecs
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_missing_protected_fields_that_are_in_behaviors>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
    It should_print_the_behaviors_containing_missing_fields = () =>
      Exception.Message.ShouldContain(typeof(BehaviorsWithProtectedFields).FullName);
    It should_print_the_missing_fields = () =>
      Exception.Message.ShouldContain("fieldThatShouldBeCopiedOverFromContext");
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_that_has_fields_typed_differently_than_needed_by_the_behavior
    : RunnerSpecs
  {
    static Exception Exception;

    Because of = () => { Exception = Catch.Exception(Run<context_with_protected_fields_having_different_types_than_in_behaviors>); };

    It should_fail = () => Exception.ShouldBeOfType<SpecificationUsageException>();
    It should_print_the_behaviors_containing_wrongly_typed_fields = () =>
      Exception.Message.ShouldContain(typeof(BehaviorsWithProtectedFields).FullName);
    It should_print_the_wrongly_typed_fields = () =>
      Exception.Message.ShouldContain("fieldThatShouldBeCopiedOverFromContext");
  }
}