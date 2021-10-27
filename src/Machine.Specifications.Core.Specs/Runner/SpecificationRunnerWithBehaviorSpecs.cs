using System;
using System.Reflection;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior : RandomRunnerSpecs
    {
        static Type context_with_behaviors;
        static Type Behaviors;

        Establish context = () =>
        {
            context_with_behaviors = GetRandom("context_with_behaviors");
            Behaviors = GetRandom("Behaviors");

            context_with_behaviors.ToDynamic().local_spec_ran = false;
            Behaviors.ToDynamic().behavior_spec_ran = false;
        };

        Because of = () =>
            Run(context_with_behaviors);

        It should_run_the_context_spec = () =>
            context_with_behaviors.ToDynamic().local_spec_ran.ShouldBeTrue();

        It should_run_the_behavior_spec = () =>
            Behaviors.ToDynamic().behavior_spec_ran.ShouldBeTrue();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_field_is_ignored : RandomRunnerSpecs
    {
        static Type context_with_behaviors_where_the_behavior_field_is_ignored;
        static Type Behaviors;

        Establish context = () =>
        {
            context_with_behaviors_where_the_behavior_field_is_ignored = GetRandom("context_with_behaviors_where_the_behavior_field_is_ignored");
            Behaviors = GetRandom("Behaviors");

            context_with_behaviors_where_the_behavior_field_is_ignored.ToDynamic().local_spec_ran = false;
            Behaviors.ToDynamic().behavior_spec_ran = false;
        };

        Because of = () =>
            Run(context_with_behaviors_where_the_behavior_field_is_ignored);

        It should_run_the_context_spec = () =>
            context_with_behaviors_where_the_behavior_field_is_ignored.ToDynamic().local_spec_ran.ShouldBeTrue();

        It should_not_run_the_behavior_spec = () =>
            Behaviors.ToDynamic().behavior_spec_ran.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_is_ignored : RandomRunnerSpecs
    {
        static Type context_with_behaviors_where_the_behavior_is_ignored;
        static Type IgnoredBehaviors;

        Establish context = () =>
        {
            context_with_behaviors_where_the_behavior_is_ignored = GetRandom("context_with_behaviors");
            IgnoredBehaviors = GetRandom("IgnoredBehaviors");

            context_with_behaviors_where_the_behavior_is_ignored.ToDynamic().local_spec_ran = false;
            IgnoredBehaviors.ToDynamic().behavior_spec_ran = false;
        };

        Because of = () =>
            Run(context_with_behaviors_where_the_behavior_is_ignored);

        It should_run_the_context_spec = () =>
            context_with_behaviors_where_the_behavior_is_ignored.ToDynamic().local_spec_ran.ShouldBeTrue();

        It should_not_run_the_behavior_spec = () =>
            IgnoredBehaviors.ToDynamic().behavior_spec_ran.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_specifications_in_a_behavior_where_the_behavior_specs_are_ignored : RandomRunnerSpecs
    {
        static Type context_with_behaviors_where_the_behavior_specs_are_ignored;
        static Type BehaviorsWithIgnoredSpec;

        Establish context = () =>
        {
            context_with_behaviors_where_the_behavior_specs_are_ignored = GetRandom("context_with_behaviors_where_the_behavior_specs_are_ignored");
            BehaviorsWithIgnoredSpec = GetRandom("BehaviorsWithIgnoredSpec");

            context_with_behaviors_where_the_behavior_specs_are_ignored.ToDynamic().local_spec_ran = false;
            BehaviorsWithIgnoredSpec.ToDynamic().behavior_spec_ran = false;
        };

        Because of = () =>
            Run(context_with_behaviors_where_the_behavior_specs_are_ignored);

        It should_run_the_context_spec = () =>
            context_with_behaviors_where_the_behavior_specs_are_ignored.ToDynamic().local_spec_ran.ShouldBeTrue();

        It should_not_run_the_behavior_spec = () =>
            BehaviorsWithIgnoredSpec.ToDynamic().behavior_spec_ran.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_nested_behaviors : RandomRunnerSpecs
    {
        static Type context_with_nested_behaviors;
        static Exception exception;

        Establish context = () =>
            context_with_nested_behaviors = GetRandom("context_with_nested_behaviors");

        Because of = () =>
            exception = Catch.Exception(() => Run(context_with_nested_behaviors));

        It should_fail = () =>
            exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_type_containing_the_nested_behaviors = () =>
            exception.Message.ShouldContain("Example.Random.BehaviorsWithNestedBehavior");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_behaviors_that_do_not_have_the_behaviors_attribute : RandomRunnerSpecs
    {
        static Type context_with_behaviors_without_behaviors_attribute;
        static Exception exception;

        Establish context = () =>
            context_with_behaviors_without_behaviors_attribute = GetRandom("context_with_behaviors_without_behaviors_attribute");

        Because of = () =>
            exception = Catch.Exception(() => Run(context_with_behaviors_without_behaviors_attribute));

        It should_fail = () =>
            exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_type_missing_the_attribute = () =>
            exception.Message.ShouldContain("Example.Random.BehaviorsWithoutBehaviorsAttribute");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_behaviors_with_establish : RandomRunnerSpecs
    {
        static Type context_with_behaviors_with_establish;
        static Exception exception;

        Establish context = () =>
            context_with_behaviors_with_establish = GetRandom("context_with_behaviors_with_establish");

        Because of = () => exception =
            Catch.Exception(() => Run(context_with_behaviors_with_establish));

        It should_fail = () =>
            exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_with_the_establish = () =>
            exception.Message.ShouldContain("Example.Random.BehaviorsWithEstablish");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_behaviors_with_because : RandomRunnerSpecs
    {
        static Type context_with_behaviors_with_because;
        static Exception exception;

        Establish context = () =>
            context_with_behaviors_with_because = GetRandom("context_with_behaviors_with_because");

        Because of = () =>
            exception = Catch.Exception(() => Run(context_with_behaviors_with_because));

        It should_fail = () =>
            exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_with_the_because = () =>
            exception.Message.ShouldContain("Example.Random.BehaviorsWithBecause");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_that_does_not_have_all_fields_needed_by_the_behavior : RandomRunnerSpecs
    {
        static Type context_missing_protected_fields_that_are_in_behaviors;
        static Exception exception;

        Establish context = () =>
            context_missing_protected_fields_that_are_in_behaviors = GetRandom("context_missing_protected_fields_that_are_in_behaviors");

        Because of = () =>
            exception = Catch.Exception(() => Run(context_missing_protected_fields_that_are_in_behaviors));

        It should_fail = () =>
            exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_containing_missing_fields = () =>
            exception.Message.ShouldContain("Example.Random.BehaviorsWithProtectedFields");

        It should_print_the_missing_fields = () =>
            exception.Message.ShouldContain("field_that_should_be_copied_over_from_context");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_that_has_fields_typed_differently_than_needed_by_the_behavior : RandomRunnerSpecs
    {
        static Type context_with_protected_fields_having_different_types_than_in_behaviors;
        static Exception exception;

        Establish context = () =>
            context_with_protected_fields_having_different_types_than_in_behaviors = GetRandom("context_with_protected_fields_having_different_types_than_in_behaviors");

        Because of = () =>
            exception = Catch.Exception(() => Run(context_with_protected_fields_having_different_types_than_in_behaviors));

        It should_fail = () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_print_the_behaviors_containing_wrongly_typed_fields = () =>
            exception.Message.ShouldContain("Example.Random.BehaviorsWithProtectedFields");

        It should_print_the_wrongly_typed_fields = () =>
            exception.Message.ShouldContain("field_that_should_be_copied_over_from_context");
    }
}
