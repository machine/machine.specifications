using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Specs.Fixtures;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Specification Runner")]
    public class when_running_a_context_with_no_specifications : RandomRunnerSpecs
    {
        static Type TestCleanupAfterEveryContext;
        static Type context_with_no_specs;

        Establish context = () =>
        {
            TestCleanupAfterEveryContext = GetRandom("TestCleanupAfterEveryContext");
            context_with_no_specs = GetRandom("context_with_no_specs");

            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRun = false;
            context_with_no_specs.ToDynamic().context_established = false;
            context_with_no_specs.ToDynamic().cleanup_occurred = false;
        };

        Because of = () =>
            Run(context_with_no_specs);

        It should_not_establish_the_context = () =>
            context_with_no_specs.ToDynamic().context_established.ShouldBeFalse();

        It should_not_cleanup = () =>
            context_with_no_specs.ToDynamic().cleanup_occurred.ShouldBeFalse();

        It should_not_perform_assembly_wide_cleanup = () =>
            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRun.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_a_ignored_specifications : RandomRunnerSpecs
    {
        static Type context_with_no_specs;
        static Type context_with_ignore_on_one_spec;
        static Type TestCleanupAfterEveryContext;

        Establish context = () =>
        {
            context_with_no_specs = GetRandom("context_with_no_specs");
            context_with_ignore_on_one_spec = GetRandom("context_with_ignore_on_one_spec");
            TestCleanupAfterEveryContext = GetRandom("TestCleanupAfterEveryContext");

            context_with_no_specs.ToDynamic().context_established = false;
            context_with_no_specs.ToDynamic().cleanup_occurred = false;
            context_with_ignore_on_one_spec.ToDynamic().ignored_spec_ran = false;
            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRun = false;
        };

        Because of = () =>
            Run(context_with_ignore_on_one_spec);

        It should_not_run_the_spec = () =>
            context_with_ignore_on_one_spec.ToDynamic().ignored_spec_ran.ShouldBeFalse();

        It should_not_establish_the_context = () =>
            context_with_ignore_on_one_spec.ToDynamic().context_established.ShouldBeFalse();

        It should_not_cleanup = () =>
            context_with_ignore_on_one_spec.ToDynamic().cleanup_occurred.ShouldBeFalse();

        It should_not_perform_assembly_wide_cleanup = () =>
            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRun.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_an_ignored_context : RandomRunnerSpecs
    {
        static Type context_with_ignore;
        static Type TestCleanupAfterEveryContext;

        Establish context = () =>
        {
            context_with_ignore = GetRandom("context_with_ignore");
            TestCleanupAfterEveryContext = GetRandom("TestCleanupAfterEveryContext");

            context_with_ignore.ToDynamic().ignored_spec_ran = false;
            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRun = false;
        };

        Because of = () =>
            Run(context_with_ignore);

        It should_not_run_the_spec = () =>
            context_with_ignore.ToDynamic().ignored_spec_ran.ShouldBeFalse();

        It should_not_establish_the_context = () =>
            context_with_ignore.ToDynamic().context_established.ShouldBeFalse();

        It should_not_cleanup = () =>
            context_with_ignore.ToDynamic().cleanup_occurred.ShouldBeFalse();

        It should_not_perform_assembly_wide_cleanup = () =>
            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRun.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_multiple_specifications : RandomRunnerSpecs
    {
        static Type context_with_multiple_specifications;
        static Type TestCleanupAfterEveryContext;

        Establish context = () =>
        {
            context_with_multiple_specifications = GetRandom("context_with_multiple_specifications");
            TestCleanupAfterEveryContext = GetRandom("TestCleanupAfterEveryContext");

            context_with_multiple_specifications.ToDynamic().establish_run_count = 0;
            context_with_multiple_specifications.ToDynamic().because_clause_run_count = 0;
            TestCleanupAfterEveryContext.ToDynamic().Reset();
        };

        Because of = () =>
            Run(context_with_multiple_specifications);

        It should_establish_the_context_once = () =>
            context_with_multiple_specifications.ToDynamic().establish_run_count.ShouldEqual(1);

        It should_invoke_the_because_clause_once = () =>
            context_with_multiple_specifications.ToDynamic().because_clause_run_count.ShouldEqual(1);

        It should_invoke_the_assembly_wide_cleanup_once = () =>
            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRunCount.ShouldEqual(1);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_multiple_specifications_and_setup_once_per_attribute : RandomRunnerSpecs
    {
        static Type context_with_multiple_specifications_and_setup_for_each;
        static Type TestCleanupAfterEveryContext;

        Establish context = () =>
        {
            context_with_multiple_specifications_and_setup_for_each = GetRandom("context_with_multiple_specifications_and_setup_for_each");
            TestCleanupAfterEveryContext = GetRandom("TestCleanupAfterEveryContext");

            context_with_multiple_specifications_and_setup_for_each.ToDynamic().establish_run_count = 0;
            context_with_multiple_specifications_and_setup_for_each.ToDynamic().because_clause_run_count = 0;
            TestCleanupAfterEveryContext.ToDynamic().Reset();
        };

        Because of = () =>
            Run(context_with_multiple_specifications_and_setup_for_each);

        It should_establish_the_context_for_each_specification = () =>
            context_with_multiple_specifications_and_setup_for_each.ToDynamic().establish_run_count.ShouldEqual(2);

        It should_invoke_the_because_clause_for_each_specification = () =>
            context_with_multiple_specifications_and_setup_for_each.ToDynamic().because_clause_run_count.ShouldEqual(2);

        It should_invoke_the_assembly_wide_cleanup_once_per_spec = () =>
            TestCleanupAfterEveryContext.ToDynamic().AfterContextCleanupRunCount.ShouldEqual(2);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_multiple_establish_clauses : FailingRunnerSpecs
    {
        static Exception exception;

        Because of = () =>
        {
            var type = Assembly.LoadFile(AssemblyPath)
                .GetType("Example.Failing.context_with_multiple_establish_clauses");

            exception = Catch.Exception(() => Run(type));
        };

        It should_fail =
            () => exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_report_the_reason =
            () => exception.Message.ShouldStartWith(
                "You cannot have more than one Establish clause in Example.Failing.context_with_multiple_establish_clauses");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_multiple_setup_clauses_and_custom_delegates : FailingRunnerSpecs
    {
        static Exception exception;

        Because of = () =>
        {
            var type = Assembly.LoadFile(AssemblyPath)
                .GetType("Example.Failing.context_with_multiple_given_clauses");

            exception = Catch.Exception(() => Run(type));
        };

        It should_fail = () =>
            exception.ShouldBeOfExactType<SpecificationUsageException>();

        It should_report_the_reason = () =>
            exception.Message.ShouldStartWith(
                "You cannot have more than one Given clause in Example.Failing.context_with_multiple_given_clauses");
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_failing_establish_clauses : RandomRunnerSpecs
    {
        static Type context_with_failing_establish;

        Establish context = () =>
            context_with_failing_establish = GetRandom("context_with_failing_establish");

        Because of = () =>
            Run(context_with_failing_establish);

        It should_fail = () =>
            testListener.LastResult.Passed.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_failing_because_clauses : RandomRunnerSpecs
    {
        static Type context_with_failing_because;

        Establish context = () =>
            context_with_failing_because = GetRandom("context_with_failing_because");

        Because of = () =>
            Run(context_with_failing_because);

        It should_fail = () =>
            testListener.LastResult.Passed.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_failing_cleanup_clause : RandomRunnerSpecs
    {
        static Exception exception;
        static Type context_with_failing_cleanup;

        Establish context = () =>
            context_with_failing_cleanup = GetRandom("context_with_failing_cleanup");

        Because of = () =>
            Run(context_with_failing_cleanup);

        It should_report_cleanup_exception = () =>
        {
            var exception = (Exception) context_with_failing_cleanup.ToDynamic().ExceptionThrownByCleanup;

            testListener
                .LastFatalError
                .Message.ShouldEqual(exception.Message);
        };
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_failing_specs : RandomRunnerSpecs
    {
        static Type context_with_failing_specs;

        Establish context = () =>
            context_with_failing_specs = GetRandom("context_with_failing_specs");

        Because of = () =>
            Run(context_with_failing_specs);

        It should_fail = () =>
            testListener.LastResult.Passed.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_an_assembly_with_no_included_contexts : RandomRunnerSpecs
    {
        static Type TestAssemblyContext;
        static DefaultRunner runner;

        Establish context = () =>
        {
            TestAssemblyContext = GetRandom("TestAssemblyContext");

            TestAssemblyContext.ToDynamic().OnAssemblyStartRun = false;
            TestAssemblyContext.ToDynamic().OnAssemblyCompleteRun = false;
            runner = new DefaultRunner(new TestListener(),
                new RunOptions(new[] { "asdfasdf" }, new string[0], new string[0]));
        };

        Because of = () =>
            runner.RunAssembly(TestAssemblyContext.GetTypeInfo().Assembly);

        It should_not_run_assembly_start = () =>
            TestAssemblyContext.ToDynamic().OnAssemblyStartRun.ShouldBeFalse();

        It should_not_run_assembly_complete = () =>
            TestAssemblyContext.ToDynamic().OnAssemblyCompleteRun.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_a_specification_with_console_output : RandomRunnerSpecs
    {
        static Type context_with_console_output;

        Establish context = () =>
            context_with_console_output = GetRandom("context_with_console_output");

        Because of = () =>
            Run(context_with_console_output);

        It should_capture_the_standard_output = () =>
            testListener.LastAssembly.CapturedOutput.ShouldEqual(string.Format(
                "Console.Out message in establish{0}" +
                "Console.Out message in because{0}" +
                "Console.Out message in spec{0}" +
                "Console.Out message in nth spec{0}" +
                "Console.Out message in cleanup{0}",
                Environment.NewLine));
    }

    [Subject("Specification Runner")]
    public class when_running_a_specification_with_error_output : RandomRunnerSpecs
    {
        static Type context_with_console_error_output;

        Establish context = () =>
            context_with_console_error_output = GetRandom("context_with_console_error_output");

        Because of = () =>
            Run(context_with_console_error_output);

        It should_capture_the_standard_error = () =>
            testListener.LastAssembly.CapturedOutput.ShouldEqual(string.Format(
                "Console.Error message in establish{0}" +
                "Console.Error message in because{0}" +
                "Console.Error message in spec{0}" +
                "Console.Error message in nth spec{0}" +
                "Console.Error message in cleanup{0}",
                Environment.NewLine));
    }

    // FIXME: Disabled due to false positives in CI
    //
    // #if !NETCORE
    // // Redirecting Debug output is not supported / doesn't work in .NET Core
    //   [Subject("Specification Runner")]
    //   public class when_running_a_specification_with_debug_output
    //     : RunnerSpecs
    //   {
    //     Because of = () =>
    //       Run<context_with_debug_output>();

    //     It should_capture_the_debug_trace =
    //       () => testListener.LastAssembly.CapturedOutput.Should().Be(String.Format("Debug.WriteLine message in establish{0}" +
    //                                                                                "Debug.WriteLine message in because{0}" +
    //                                                                                "Debug.WriteLine message in spec{0}" +
    //                                                                                "Debug.WriteLine message in nth spec{0}" +
    //                                                                                "Debug.WriteLine message in cleanup{0}",
    //                                                                                Environment.NewLine));
    //   }
    // #endif

    [Subject("Specification Runner")]
    public class when_running_a_specification_with_console_output_and_foreach : RandomRunnerSpecs
    {
        static Type context_with_console_output_and_setup_for_each;

        Establish context = () =>
            context_with_console_output_and_setup_for_each = GetRandom("context_with_console_output_and_setup_for_each");

        Because of = () =>
            Run(context_with_console_output_and_setup_for_each);

        It should_capture_the_standard_output = () =>
            testListener.LastAssembly.CapturedOutput.ShouldEqual(string.Format(
                "Console.Out message in establish{0}" +
                "Console.Out message in because{0}" +
                "Console.Out message in spec{0}" +
                "Console.Out message in cleanup{0}" +
                "Console.Out message in establish{0}" +
                "Console.Out message in because{0}" +
                "Console.Out message in nth spec{0}" +
                "Console.Out message in cleanup{0}",
                Environment.NewLine));
    }

    [Subject("Specification Runner")]
    public class when_running_a_specification_that_throws_an_exception_with_an_inner_exception : RandomRunnerSpecs
    {
        static Type context_with_inner_exception;

        Establish context = () =>
            context_with_inner_exception = GetRandom("context_with_inner_exception");

        Because of = () =>
            Run(context_with_inner_exception);

        It should_include_the_inner_exception_in_the_result = () =>
            testListener.LastResult.Exception.ToString().ShouldContain("INNER123");
    }

    [Subject("Specification Runner")]
    public class when_running_a_behavior : RandomRunnerSpecs
    {
        static Type Behaviors;

        Establish context = () =>
        {
            Behaviors = GetRandom("Behaviors");

            Behaviors.ToDynamic().behavior_spec_ran = false;
        };

        Because of = () =>
            Run(Behaviors);

        It should_not_run_the_behavior_specs = () =>
            Behaviors.ToDynamic().behavior_spec_ran.ShouldBeFalse();
    }

    [Subject("Specification Runner")]
    public class when_running_with_tag : RandomRunnerSpecs
    {
        static DefaultRunner runner;
        static TestListener testListener;

        static Type TaggedCleanup;
        static Type UntaggedCleanup;
        static Type TaggedAssemblyContext;
        static Type UntaggedAssemblyContext;
        static Type TaggedResultSupplementer;
        static Type UntaggedResultSupplementer;
        static Type context_with_multiple_specifications;

        Establish context = () =>
        {
            TaggedCleanup = GetRandom("TaggedCleanup");
            UntaggedCleanup = GetRandom("UntaggedCleanup");
            TaggedAssemblyContext = GetRandom("TaggedAssemblyContext");
            UntaggedAssemblyContext = GetRandom("UntaggedAssemblyContext");
            TaggedResultSupplementer = GetRandom("TaggedResultSupplementer");
            UntaggedResultSupplementer = GetRandom("UntaggedResultSupplementer");
            context_with_multiple_specifications = GetRandom("context_with_multiple_specifications");

            TaggedCleanup.ToDynamic().Reset();
            UntaggedCleanup.ToDynamic().Reset();
            TaggedAssemblyContext.ToDynamic().Reset();
            UntaggedAssemblyContext.ToDynamic().Reset();
            TaggedResultSupplementer.ToDynamic().Reset();
            UntaggedResultSupplementer.ToDynamic().Reset();

            testListener = new TestListener();

            var options = new RunOptions(new[] { "foobar" }, new string[] { }, new string[0]);

            runner = new DefaultRunner(testListener, options);
        };

        Because of = () =>
            runner.RunMember(context_with_multiple_specifications.GetTypeInfo().Assembly,
                context_with_multiple_specifications.GetTypeInfo());

        It should_run_untagged_assembly_context = () =>
            UntaggedAssemblyContext.ToDynamic().OnAssemblyStartRun.ShouldBeTrue();

        It should_run_tagged_assembly_context = () =>
            TaggedAssemblyContext.ToDynamic().OnAssemblyStartRun.ShouldBeTrue();

        It should_run_untagged_assembly_context_complete = () =>
            UntaggedAssemblyContext.ToDynamic().OnAssemblyCompleteRun.ShouldBeTrue();

        It should_run_tagged_assembly_context_complete = () =>
            TaggedAssemblyContext.ToDynamic().OnAssemblyCompleteRun.ShouldBeTrue();

        It should_run_untagged_global_cleanup = () =>
            UntaggedCleanup.ToDynamic().AfterContextCleanupRunCount.ShouldBeGreaterThan(0);

        It should_run_tagged_global_cleanup = () =>
            TaggedCleanup.ToDynamic().AfterContextCleanupRunCount.ShouldBeGreaterThan(0);

        It should_run_tagged_result_supplementer = () =>
            TaggedResultSupplementer.ToDynamic().SupplementResultRun.ShouldBeTrue();

        It should_run_untagged_result_supplementer = () =>
            UntaggedResultSupplementer.ToDynamic().SupplementResultRun.ShouldBeTrue();
    }

    [Subject("Specification Runner")]
    public class when_running_with_empty_filters : RandomRunnerSpecs
    {
        static DefaultRunner runner;
        static TestListener testListener;

        static Type context_with_multiple_specifications;
        static Type context_with_duplicate_tags;

        Establish context = () =>
        {
            context_with_multiple_specifications = GetRandom("context_with_multiple_specifications");
            context_with_duplicate_tags = GetRandom("context_with_duplicate_tags");

            testListener = new TestListener();
            var options = new RunOptions(new string[] { }, new string[] { }, new string[0]);

            runner = new DefaultRunner(testListener, options);
        };

        Because of = () =>
        {
            runner.RunMember(context_with_multiple_specifications.GetTypeInfo().Assembly,
                context_with_multiple_specifications.GetTypeInfo());
            runner.RunMember(context_with_duplicate_tags.GetTypeInfo().Assembly,
                context_with_duplicate_tags.GetTypeInfo());
        };

        It should_run_everything = () =>
            testListener.SpecCount.ShouldEqual(3);
    }

    [Subject("Specification Runner")]
    public class when_running_with_context_filters : RandomRunnerSpecs
    {
        static DefaultRunner runner;
        static TestListener testListener;

        static Type context_with_multiple_specifications;
        static Type context_with_duplicate_tags;

        Establish context = () =>
        {
            context_with_multiple_specifications = GetRandom("context_with_multiple_specifications");
            context_with_duplicate_tags = GetRandom("context_with_duplicate_tags");

            testListener = new TestListener();
            var options = new RunOptions(new string[] { }, new string[] { },
                new[] { "Example.Random.context_with_multiple_specifications" });

            runner = new DefaultRunner(testListener, options);
        };

        Because of = () =>
        {
            runner.RunMember(context_with_multiple_specifications.GetTypeInfo().Assembly,
                context_with_multiple_specifications.GetTypeInfo());
            runner.RunMember(context_with_duplicate_tags.GetTypeInfo().Assembly,
                context_with_duplicate_tags.GetTypeInfo());
        };

        It should_run_included_contexts_only = () =>
            testListener.SpecCount.ShouldEqual(2);
    }

    [Subject("Specification Runner")]
    public class when_running_with_specification_filters : RandomRunnerSpecs
    {
        static DefaultRunner runner;
        static TestListener testListener;

        static Type context_with_multiple_specifications;
        static Type context_with_duplicate_tags;

        Establish context = () =>
        {
            context_with_multiple_specifications = GetRandom("context_with_multiple_specifications");
            context_with_duplicate_tags = GetRandom("context_with_duplicate_tags");

            testListener = new TestListener();
            var options = new RunOptions(new string[] { },
                new string[] { },
                new[]
                {
                    "Example.Random.context_with_multiple_specifications",
                    "Example.Random.context_with_duplicate_tags"
                });

            runner = new DefaultRunner(testListener, options);
        };

        Because of = () =>
        {
            runner.RunMember(context_with_multiple_specifications.GetTypeInfo().Assembly,
                context_with_multiple_specifications.GetTypeInfo());
            runner.RunMember(context_with_duplicate_tags.GetTypeInfo().Assembly,
                context_with_duplicate_tags.GetTypeInfo());
        };

        It should_run_included_specifications_only = () =>
            testListener.SpecCount.ShouldEqual(3);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_public_Its : RandomRunnerSpecs
    {
        static Type context_with_public_It_field;

        Establish context = () =>
            context_with_public_It_field = GetRandom("context_with_public_It_field");

        Because of = () =>
            Run(context_with_public_It_field);

        It should_succeed = () =>
            testListener.SpecCount.ShouldEqual(1);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_protected_Its : RandomRunnerSpecs
    {
        static Type context_with_protected_It_field;

        Establish context = () =>
            context_with_protected_It_field = GetRandom("context_with_protected_It_field");

        Because of = () =>
            Run(context_with_protected_It_field);

        It should_succeed = () =>
            testListener.SpecCount.ShouldEqual(1);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_internal_Its : RandomRunnerSpecs
    {
        static Type context_with_internal_It_field;

        Establish context = () =>
            context_with_internal_It_field = GetRandom("context_with_internal_It_field");

        Because of = () =>
            Run(context_with_internal_It_field);

        It should_succeed = () =>
            testListener.SpecCount.ShouldEqual(1);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_public_Behaves_like : RandomRunnerSpecs
    {
        static Type context_with_public_Behaves_like_field;

        Establish context = () =>
            context_with_public_Behaves_like_field = GetRandom("context_with_public_Behaves_like_field");

        Because of = () =>
            Run(context_with_public_Behaves_like_field);

        It should_succeed = () =>
            testListener.SpecCount.ShouldEqual(1);
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_with_nonprivate_framework_fields : RandomRunnerSpecs
    {
        static Type context_with_nonprivate_framework_fields;

        Establish context = () =>
            context_with_nonprivate_framework_fields = GetRandom("context_with_nonprivate_framework_fields");

        Because of = () =>
            Run(context_with_nonprivate_framework_fields);

        It should_succeed = () =>
            testListener.LastResult.Passed.ShouldBeTrue();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_inside_a_static_class : RandomRunnerSpecs
    {
        static Type when_a_context_is_nested_inside_a_static_class;

        Establish context = () =>
            when_a_context_is_nested_inside_a_static_class = GetRandom("StaticContainer+when_a_context_is_nested_inside_a_static_class");

        Because of = () =>
            Run(when_a_context_is_nested_inside_a_static_class);

        It should_succeed = () =>
            testListener.LastResult.Passed.ShouldBeTrue();
    }

    [Subject("Specification Runner")]
    public class when_running_a_context_inside_a_static_class_that_is_nested_in_a_nonstatic_class : RandomRunnerSpecs
    {
        static Type when_a_context_is_nested_inside_a_static_class_that_is_nested_inside_a_class;

        Establish context = () =>
            when_a_context_is_nested_inside_a_static_class_that_is_nested_inside_a_class = GetRandom(
                    "NonStaticContainer+StaticContainer+when_a_context_is_nested_inside_a_static_class_that_is_nested_inside_a_class");

        Because of = () =>
            Run(when_a_context_is_nested_inside_a_static_class_that_is_nested_inside_a_class);

        It should_succeed = () =>
            testListener.LastResult.Passed.ShouldBeTrue();
    }

    [Subject("Specification Runner")]
    public class when_running_a_single_spec_out_of_a_large_number_of_specifications : RunnerSpecs
    {
        static Type when_a_context_has_many_specifications;
        static TimeSpan elapsed { get; set; }

        Establish context = () =>
        {
            using (var compiler = new CompileContext())
            {
                var assemblyPath = compiler.Compile(LargeFixture.CreateCode(10000));
                var assembly = Assembly.LoadFile(assemblyPath);

                when_a_context_has_many_specifications = assembly.GetType("Example.Large.when_there_are_many_contexts");
            }
        };

        Because of = () =>
        {
            var runner = new DefaultRunner(testListener, new RunOptions(
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                new[] {when_a_context_has_many_specifications.FullName})
            );

            var sw = Stopwatch.StartNew();
            runner.RunAssembly(when_a_context_has_many_specifications.Assembly);
            sw.Stop();
            elapsed = sw.Elapsed;
        };

        It should_run_the_single_specification = () =>
        {
            testListener.SpecCount.ShouldEqual(1);
        };

        It should_run_in_a_reasonable_period_of_time = () =>
        {
            elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(1));
        };
    }

    public class RandomRunnerSpecs : RunnerSpecs
    {
        static CompileContext compiler;
        static Assembly assembly;

        Establish context = () =>
        {
            compiler = new CompileContext();

            var assemblyPath = compiler.Compile(RandomFixture.Code);
            assembly = Assembly.LoadFile(assemblyPath);
        };

        Cleanup after = () =>
            compiler.Dispose();

        protected static Assembly GetAssembly()
        {
            return assembly;
        }

        protected static Type GetRandom(string value)
        {
            return assembly.GetType($"Example.Random.{value}");
        }

        protected static Type GetFramework(string value)
        {
            return assembly.GetType($"Machine.Specifications.{value}");
        }
    }

    public class ExampleRunnerSpecs : RunnerSpecs
    {
        static CompileContext compiler;

        protected static string AssemblyPath;

        Establish context = () =>
        {
            compiler = new CompileContext();
            AssemblyPath = compiler.Compile(ExampleFixture.Code);
        };

        Cleanup after = () =>
            compiler.Dispose();
    }

    public class FailingRunnerSpecs : RunnerSpecs
    {
        static CompileContext compiler;

        protected static string AssemblyPath;

        Establish context = () =>
        {
            compiler = new CompileContext();
            AssemblyPath = compiler.Compile(FailingFixture.Code);
        };

        Cleanup after = () =>
            compiler.Dispose();
    }

    public class RunnerSpecs
    {
        static DefaultRunner runner;
        protected static TestListener testListener;

        Establish context = () =>
        {
            testListener = new TestListener();
            runner = new DefaultRunner(testListener, RunOptions.Default);
        };

        public static void Run<T>()
        {
            runner.RunMember(typeof(T).GetTypeInfo().Assembly, typeof(T).GetTypeInfo());
        }

        public static void Run(Type type)
        {
            runner.RunMember(type.GetTypeInfo().Assembly, type.GetTypeInfo());
        }
    }

    public class TestListener : ISpecificationRunListener
    {
        public int SpecCount;

        public AssemblyInfo LastAssembly { get; private set; }
        public ContextInfo LastContext { get; private set; }
        public SpecificationInfo LastSpecification { get; private set; }
        public ExceptionResult LastFatalError { get; private set; }
        public Result LastResult { get; private set; }

        public void OnRunStart()
        {
            LastAssembly = null;
            LastContext = null;
            LastResult = null;
        }

        public void OnRunEnd()
        {
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            LastAssembly = assembly;
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
            LastContext = context;
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            LastSpecification = specification;
            LastResult = result;
            SpecCount++;
        }

        public void OnFatalError(ExceptionResult exception)
        {
            LastFatalError = exception;
        }
    }
}
