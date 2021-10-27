using System;
using System.Reflection;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Specification Runner")]
    public class when_running_a_context_with_inherited_specifications : RandomRunnerSpecs
    {
        static Type context_that_inherits;
        static Type context_with_inherited_specifications;

        Establish context = () =>
        {
            context_that_inherits = GetRandom("context_that_inherits");
            context_with_inherited_specifications = GetRandom("context_with_inherited_specifications");

            context_that_inherits.ToDynamic().base_establish_run_count = 0;
            context_with_inherited_specifications.ToDynamic().because_clause_run_count = 0;
            context_with_inherited_specifications.ToDynamic().establish_run_count = 0;
        };

        Because of = () =>
            Run(context_with_inherited_specifications);

        It should_establish_the_context_once = () =>
            context_with_inherited_specifications.ToDynamic().because_clause_run_count.ShouldEqual(1);

        It should_invoke_the_because_clause_once = () =>
            context_with_inherited_specifications.ToDynamic().establish_run_count.ShouldEqual(1);

        It should_invoke_the_because_clause_from_the_base_class_once = () =>
            context_that_inherits.ToDynamic().base_establish_run_count.ShouldEqual(1);

        It should_detect_two_specs = () =>
            testListener.SpecCount.ShouldEqual(2);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_inherited_specifications_and_setup_once_per_attribute : RandomRunnerSpecs
    {
        static Type context_that_inherits;
        static Type context_with_inherited_specifications_and_setup_for_each;

        Establish context = () =>
        {
            context_that_inherits = GetRandom("context_that_inherits");
            context_with_inherited_specifications_and_setup_for_each = GetRandom("context_with_inherited_specifications_and_setup_for_each");

            context_that_inherits.ToDynamic().base_establish_run_count = 0;
            context_with_inherited_specifications_and_setup_for_each.ToDynamic().because_clause_run_count = 0;
            context_with_inherited_specifications_and_setup_for_each.ToDynamic().establish_run_count = 0;
        };

        Because of = () =>
            Run(context_with_inherited_specifications_and_setup_for_each);

        It should_establish_the_context_twice = () =>
            context_with_inherited_specifications_and_setup_for_each.ToDynamic().because_clause_run_count.ShouldEqual(2);

        It should_invoke_the_because_clause_twice = () =>
            context_with_inherited_specifications_and_setup_for_each.ToDynamic().establish_run_count.ShouldEqual(2);

        It should_invoke_the_because_clause_from_the_base_class_twice = () =>
            context_that_inherits.ToDynamic().base_establish_run_count.ShouldEqual(2);

        It should_detect_two_specs = () =>
            testListener.SpecCount.ShouldEqual(2);
    }
}
