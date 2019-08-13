using System;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior : RunnerSpecs
    {
        Establish context = () =>
        {
            context_with_behaviors.local_spec_ran = false;
            Behaviors.behavior_spec_ran = false;
        };

        Because of = Run<context_with_behaviors>;

        It should_run_the_context_spec = () =>
            context_with_behaviors.local_spec_ran.ShouldBeTrue();

        It should_run_the_behavior_spec = () =>
            Behaviors.behavior_spec_ran.ShouldBeTrue();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_field_is_ignored : RunnerSpecs
    {
        Establish context = () =>
        {
            context_with_behaviors_where_the_behavior_field_is_ignored.local_spec_ran = false;
            Behaviors.behavior_spec_ran = false;
        };

        Because of = Run<context_with_behaviors_where_the_behavior_field_is_ignored>;

        It should_run_the_context_spec = () =>
            context_with_behaviors_where_the_behavior_field_is_ignored.local_spec_ran.ShouldBeTrue();

        It should_not_run_the_behavior_spec = () => Behaviors.behavior_spec_ran.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_is_ignored : RunnerSpecs
    {
        Establish context = () =>
        {
            context_with_behaviors_where_the_behavior_is_ignored.local_spec_ran = false;
            IgnoredBehaviors.behavior_spec_ran = false;
        };

        Because of = Run<context_with_behaviors_where_the_behavior_is_ignored>;

        It should_run_the_context_spec = () =>
            context_with_behaviors_where_the_behavior_is_ignored.local_spec_ran.ShouldBeTrue();

        It should_not_run_the_behavior_spec = () => IgnoredBehaviors.behavior_spec_ran.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_specs_are_ignored : RunnerSpecs
    {
        Establish context = () =>
        {
            context_with_behaviors_where_the_behavior_specs_are_ignored.local_spec_ran = false;
            BehaviorsWithIgnoredSpec.behavior_spec_ran = false;
        };

        Because of = Run<context_with_behaviors_where_the_behavior_specs_are_ignored>;

        It should_run_the_context_spec = () =>
            context_with_behaviors_where_the_behavior_specs_are_ignored.local_spec_ran.ShouldBeTrue();

        It should_not_run_the_behavior_spec = () => BehaviorsWithIgnoredSpec.behavior_spec_ran.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_nested_behaviors : RunnerSpecs
    {
        static Exception exception;

        Because of = () => { exception = Catch.Exception(Run<context_with_nested_behaviors>); };

        It should_fail = () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_type_containing_the_nested_behaviors = () =>
            exception.Message.ShouldContain(typeof(BehaviorsWithNestedBehavior).FullName);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_behaviors_that_do_not_have_the_behaviors_attribute : RunnerSpecs
    {
        static Exception exception;

        Because of = () => { exception = Catch.Exception(Run<context_with_behaviors_without_behaviors_attribute>); };

        It should_fail = () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_type_missing_the_attribute = () =>
            exception.Message.ShouldContain(typeof(BehaviorsWithoutBehaviorsAttribute).FullName);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_behaviors_with_establish : RunnerSpecs
    {
        static Exception exception;

        Because of = () => { exception = Catch.Exception(Run<context_with_behaviors_with_establish>); };

        It should_fail = () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_with_the_establish = () =>
            exception.Message.ShouldContain(typeof(BehaviorsWithEstablish).FullName);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_behaviors_with_because : RunnerSpecs
    {
        static Exception exception;

        Because of = () => { exception = Catch.Exception(Run<context_with_behaviors_with_because>); };

        It should_fail = () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_with_the_because = () =>
            exception.Message.ShouldContain(typeof(BehaviorsWithBecause).FullName);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_that_does_not_have_all_fields_needed_by_the_behavior : RunnerSpecs
    {
        static Exception exception;

        Because of = () =>
        {
            exception = Catch.Exception(Run<context_missing_protected_fields_that_are_in_behaviors>);
        };

        It should_fail = () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_containing_missing_fields = () =>
            exception.Message.ShouldContain(typeof(BehaviorsWithProtectedFields).FullName);

        It should_print_the_missing_fields = () =>
            exception.Message.ShouldContain("field_that_should_be_copied_over_from_context");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_that_has_fields_typed_differently_than_needed_by_the_behavior : RunnerSpecs
    {
        static Exception exception;

        Because of = () =>
        {
            exception = Catch.Exception(
                Run<context_with_protected_fields_having_different_types_than_in_behaviors>);
        };

        It should_fail = () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_containing_wrongly_typed_fields = () =>
            exception.Message.ShouldContain(typeof(BehaviorsWithProtectedFields).FullName);

        It should_print_the_wrongly_typed_fields = () =>
            exception.Message.ShouldContain("field_that_should_be_copied_over_from_context");
    }
}
