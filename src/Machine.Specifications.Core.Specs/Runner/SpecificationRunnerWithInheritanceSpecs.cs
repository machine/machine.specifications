using System;
using System.Reflection;
using Example.Random;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Specification Runner")]
    class when_running_a_context_with_inherited_specifications : RunnerSpecs
    {
        Establish context = () =>
        {
            context_that_inherits.base_establish_run_count = 0;
            context_with_inherited_specifications.because_clause_run_count = 0;
            context_with_inherited_specifications.establish_run_count = 0;
        };

        Because of = () =>
            Run<context_with_inherited_specifications>();

        It should_establish_the_context_once = () =>
            context_with_inherited_specifications.because_clause_run_count.ShouldEqual(1);

        It should_invoke_the_because_clause_once = () =>
            context_with_inherited_specifications.establish_run_count.ShouldEqual(1);

        It should_invoke_the_because_clause_from_the_base_class_once = () =>
            context_that_inherits.base_establish_run_count.ShouldEqual(1);

        It should_detect_two_specs = () =>
            testListener.SpecCount.ShouldEqual(2);
    }

    [Subject("Specification Runner")]
    class when_running_a_context_with_inherited_specifications_and_setup_once_per_attribute : RunnerSpecs
    {
        Establish context = () =>
        {
            context_that_inherits.base_establish_run_count = 0;
            context_with_inherited_specifications_and_setup_for_each.because_clause_run_count = 0;
            context_with_inherited_specifications_and_setup_for_each.establish_run_count = 0;
        };

        Because of = () =>
            Run<context_with_inherited_specifications_and_setup_for_each>();

        It should_establish_the_context_twice = () =>
            context_with_inherited_specifications_and_setup_for_each.because_clause_run_count.ShouldEqual(2);

        It should_invoke_the_because_clause_twice = () =>
            context_with_inherited_specifications_and_setup_for_each.establish_run_count.ShouldEqual(2);

        It should_invoke_the_because_clause_from_the_base_class_twice = () =>
            context_that_inherits.base_establish_run_count.ShouldEqual(2);

        It should_detect_two_specs = () =>
            testListener.SpecCount.ShouldEqual(2);
    }
}
