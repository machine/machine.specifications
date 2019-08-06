using Machine.Specifications;

namespace Example.Random
{
    [Tags(tag.example, "behavior usage")]
    public class context_with_behaviors
    {
        public static bool local_spec_ran;

        It should_run = () => local_spec_ran = true;

        Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example, "behavior usage")]
    public class second_context_with_behaviors
    {
        Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_field_is_ignored
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        [Ignore("example reason")]
        Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_is_ignored
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<IgnoredBehaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_specs_are_ignored
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithIgnoredSpec> behavior;
    }

    [Tags(tag.example)]
    public class context_with_nested_behaviors
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithNestedBehavior> behavior_with_nested_behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_without_behaviors_attribute
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithoutBehaviorsAttribute> behavior_without_behavior_attribute;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_with_establish
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithEstablish> behavior_with_establish;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_with_because
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithBecause> behavior_with_because;
    }

    [Tags(tag.example)]
    public class context_missing_protected_fields_that_are_in_behaviors
    {
        public static bool local_spec_ran;

        It should_not_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithProtectedFields> behavior_with_protected_fields;
    }

    [Tags(tag.example)]
    public class context_with_protected_fields_having_different_types_than_in_behaviors
    {
        public static bool local_spec_ran;
        protected static bool field_that_should_be_copied_over_from_context;

        It should_not_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithProtectedFields> behavior_with_protected_fields;
    }

    [Behaviors]
    public class Behaviors
    {
        public static bool behavior_spec_ran;

        It should_run_if_behavior_is_not_ignored = () =>
            behavior_spec_ran = true;
    }

    [Ignore("example reason")]
    [Behaviors]
    public class IgnoredBehaviors
    {
        public static bool behavior_spec_ran;

        It should_not_run = () =>
            behavior_spec_ran = true;
    }

    [Behaviors]
    public class BehaviorsWithIgnoredSpec
    {
        public static bool behavior_spec_ran;

        [Ignore("example reason")]
        It should_not_run = () =>
            behavior_spec_ran = true;
    }

    [Behaviors]
    public class BehaviorsWithNestedBehavior
    {
        Behaves_like<object> disallowed_nested_behavior;
    }

    public class BehaviorsWithoutBehaviorsAttribute
    {
        public static bool behavior_spec_ran;

        It should_not_run = () =>
            behavior_spec_ran = true;
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

    [Behaviors]
    public class BehaviorsWithProtectedFields
    {
        public static bool behavior_spec_ran;
        protected static int field_that_should_be_copied_over_from_context;

        It should_not_run = () =>
            behavior_spec_ran = true;
    }
}
