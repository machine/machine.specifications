using System;
using System.Reflection;
using Example;
using Example.Failing;
using Example.Random;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

using TestAssemblyContext = Example.TestAssemblyContext;

namespace Machine.Specifications.Specs.Runner
{
  [Subject("Specification Runner")]
  public class when_running_a_context_with_no_specifications
    : RunnerSpecs
  {
    Establish context = () =>
    {
      TestCleanupAfterEveryContext.AfterContextCleanupRun = false;
      context_with_no_specs.context_established = false;
      context_with_no_specs.cleanup_occurred = false;
    };

    Because of = () =>
      Run<context_with_no_specs>();

    It should_not_establish_the_context = () =>
      context_with_no_specs.context_established.ShouldBeFalse();

    It should_not_cleanup = () =>
      context_with_no_specs.cleanup_occurred.ShouldBeFalse();

    It should_not_perform_assembly_wide_cleanup = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_a_ignored_specifications
    : RunnerSpecs
  {
    Establish context = () =>
    {
      context_with_no_specs.context_established = false;
      context_with_no_specs.cleanup_occurred = false;
      context_with_ignore_on_one_spec.ignored_spec_ran = false;
      TestCleanupAfterEveryContext.AfterContextCleanupRun = false;
    };

    Because of = () =>
      Run<context_with_ignore_on_one_spec>();

    It should_not_run_the_spec = () =>
      context_with_ignore_on_one_spec.ignored_spec_ran.ShouldBeFalse();

    It should_not_establish_the_context = () =>
      context_with_ignore_on_one_spec.context_established.ShouldBeFalse();

    It should_not_cleanup = () =>
      context_with_ignore_on_one_spec.cleanup_occurred.ShouldBeFalse();

    It should_not_perform_assembly_wide_cleanup = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_an_ignored_context
    : RunnerSpecs
  {
    Establish context = () =>
    {
      context_with_ignore.ignored_spec_ran = false;
      TestCleanupAfterEveryContext.AfterContextCleanupRun = false;
    };

    Because of = () =>
      Run<context_with_ignore>();

    It should_not_run_the_spec = () =>
      context_with_ignore.ignored_spec_ran.ShouldBeFalse();

    It should_not_establish_the_context = () =>
      context_with_ignore.context_established.ShouldBeFalse();

    It should_not_cleanup = () =>
      context_with_ignore.cleanup_occurred.ShouldBeFalse();

    It should_not_perform_assembly_wide_cleanup = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_specifications
    : RunnerSpecs
  {
    Establish context = () =>
    {
      context_with_multiple_specifications.establish_run_count = 0;
      context_with_multiple_specifications.because_clause_run_count = 0;
      TestCleanupAfterEveryContext.Reset();
    };

    Because of = () =>
      Run<context_with_multiple_specifications>();

    It should_establish_the_context_once = () =>
      context_with_multiple_specifications.establish_run_count.ShouldEqual(1);

    It should_invoke_the_because_clause_once = () =>
      context_with_multiple_specifications.because_clause_run_count.ShouldEqual(1);

    It should_invoke_the_assembly_wide_cleanup_once = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRunCount.ShouldEqual(1);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_specifications_and_setup_once_per_attribute
    : RunnerSpecs
  {
    Establish context = () =>
    {
      context_with_multiple_specifications_and_setup_for_each.establish_run_count = 0;
      context_with_multiple_specifications_and_setup_for_each.because_clause_run_count = 0;
      TestCleanupAfterEveryContext.Reset();
    };

    Because of = () =>
      Run<context_with_multiple_specifications_and_setup_for_each>();

    It should_establish_the_context_for_each_specification = () =>
      context_with_multiple_specifications_and_setup_for_each.establish_run_count.ShouldEqual(2);

    It should_invoke_the_because_clause_for_each_specification = () =>
      context_with_multiple_specifications_and_setup_for_each.because_clause_run_count.ShouldEqual(2);

    It should_invoke_the_assembly_wide_cleanup_once_per_spec = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRunCount.ShouldEqual(2);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_establish_clauses
    : RunnerSpecs
  {
    static Exception exception;

    Because of =
      () =>
      {
        exception = Catch.Exception(Run<context_with_multiple_establish_clauses>);
      };

    It should_fail =
      () => exception.ShouldBeOfExactType<SpecificationUsageException>();

    It should_report_the_reason =
      () => exception.Message.ShouldStartWith("You cannot have more than one Establish clause in Example.Failing.context_with_multiple_establish_clauses");
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_setup_clauses_and_custom_delegates
    : RunnerSpecs
  {
    static Exception exception;

    Because of =
      () => exception = Catch.Exception(Run<context_with_multiple_given_clauses>);

    It should_fail =
      () => exception.ShouldBeOfExactType<SpecificationUsageException>();

    It should_report_the_reason =
      () => exception.Message.ShouldStartWith("You cannot have more than one Given clause in Example.Failing.context_with_multiple_given_clauses");
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_failing_establish_clauses
    : RunnerSpecs
  {
    Because of = Run<context_with_failing_establish>;

    It should_fail = () =>
      testListener.LastResult.Passed.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_failing_because_clauses
    : RunnerSpecs
  {
    Because of = Run<context_with_failing_because>;

    It should_fail = () =>
      testListener.LastResult.Passed.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_failing_cleanup_clause
    : RunnerSpecs
  {
    static Exception exception;

    Because of = Run<context_with_failing_cleanup>;

    It should_report_cleanup_exception = () =>
      testListener
        .LastFatalError
#if !CLEAN_EXCEPTION_STACK_TRACE
        .InnerExceptionResult
#endif
        .Message.ShouldEqual(context_with_failing_cleanup.ExceptionThrownByCleanup.Message);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_failing_specs
  : RunnerSpecs
  {
    Because of = Run<context_with_failing_specs>;

    It should_fail = () =>
      testListener.LastResult.Passed.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_an_assembly_with_no_included_contexts
  {
    static DefaultRunner runner;

    Establish context = () =>
    {
      TestAssemblyContext.OnAssemblyStartRun = false;
      TestAssemblyContext.OnAssemblyCompleteRun = false;
      runner = new DefaultRunner(new TestListener(), new RunOptions(new[] { "asdfasdf" }, new string[0], new string[0]));
    };

    Because of = () =>
      runner.RunAssembly(typeof(TestAssemblyContext).GetTypeInfo().Assembly);

    It should_not_run_assembly_start = () =>
      TestAssemblyContext.OnAssemblyStartRun.ShouldBeFalse();

    It should_not_run_assembly_complete = () =>
      TestAssemblyContext.OnAssemblyCompleteRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_specification_with_console_output
    : RunnerSpecs
  {
    Because of = () =>
      Run<context_with_console_output>();

    It should_capture_the_standard_output =
      () => testListener.LastAssembly.CapturedOutput.ShouldEqual(String.Format("Console.Out message in establish{0}" +
                                                                               "Console.Out message in because{0}" +
                                                                               "Console.Out message in spec{0}" +
                                                                               "Console.Out message in nth spec{0}" +
                                                                               "Console.Out message in cleanup{0}",
                                                                               Environment.NewLine));
  }

  [Subject("Specification Runner")]
  public class when_running_a_specification_with_error_output
    : RunnerSpecs
  {
    Because of = () =>
      Run<context_with_console_error_output>();

    It should_capture_the_standard_error =
      () => testListener.LastAssembly.CapturedOutput.ShouldEqual(String.Format("Console.Error message in establish{0}" +
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
  public class when_running_a_specification_with_console_output_and_foreach
    : RunnerSpecs
  {
    Because of = () =>
      Run<context_with_console_output_and_setup_for_each>();

    It should_capture_the_standard_output =
      () => testListener.LastAssembly.CapturedOutput.ShouldEqual(String.Format("Console.Out message in establish{0}" +
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
  public class when_running_a_specification_that_throws_an_exception_with_an_inner_exception
    : RunnerSpecs
  {
    Because of = () =>
      Run<context_with_inner_exception>();

    It should_include_the_inner_exception_in_the_result = () =>
      testListener.LastResult.Exception.ToString().ShouldContain("INNER123");
  }

  [Subject("Specification Runner")]
  public class when_running_a_behavior
    : RunnerSpecs
  {
    Establish context = () =>
    {
      Behaviors.behavior_spec_ran = false;
    };

    Because of = () =>
      Run<Behaviors>();

    It should_not_run_the_behavior_specs = () =>
      Behaviors.behavior_spec_ran.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_with_tag
  {
    Establish context = () =>
    {
      TaggedCleanup.Reset();
      UntaggedCleanup.Reset();
      TaggedAssemblyContext.Reset();
      UntaggedAssemblyContext.Reset();
      TaggedResultSupplementer.Reset();
      UntaggedResultSupplementer.Reset();

      testListener = new TestListener();
      var options = new RunOptions(new string[] { "foobar" }, new string[] { }, new string[0]);

      runner = new DefaultRunner(testListener, options);
    };

    Because of = () =>
      runner.RunMember(typeof(context_with_multiple_specifications).GetTypeInfo().Assembly,
                       typeof(context_with_multiple_specifications).GetTypeInfo());

    It should_run_untagged_assembly_context = () =>
      UntaggedAssemblyContext.OnAssemblyStartRun.ShouldBeTrue();

    It should_run_tagged_assembly_context = () =>
      TaggedAssemblyContext.OnAssemblyStartRun.ShouldBeTrue();

    It should_run_untagged_assembly_context_complete = () =>
      UntaggedAssemblyContext.OnAssemblyCompleteRun.ShouldBeTrue();

    It should_run_tagged_assembly_context_complete = () =>
      TaggedAssemblyContext.OnAssemblyCompleteRun.ShouldBeTrue();

    It should_run_untagged_global_cleanup = () =>
      UntaggedCleanup.AfterContextCleanupRunCount.ShouldBeGreaterThan(0);

    It should_run_tagged_global_cleanup = () =>
      TaggedCleanup.AfterContextCleanupRunCount.ShouldBeGreaterThan(0);

    It should_run_tagged_result_supplementer = () =>
      TaggedResultSupplementer.SupplementResultRun.ShouldBeTrue();

    It should_run_untagged_result_supplementer = () =>
      UntaggedResultSupplementer.SupplementResultRun.ShouldBeTrue();

    static DefaultRunner runner;
    static TestListener testListener;
  }

  [Subject("Specification Runner")]
  public class when_running_with_empty_filters
  {
    Establish context = () =>
    {
      testListener = new TestListener();
      var options = new RunOptions(new string[] { }, new string[] { }, new string[0]);

      runner = new DefaultRunner(testListener, options);
    };

    Because of = () =>
      {
        runner.RunMember(typeof(context_with_multiple_specifications).GetTypeInfo().Assembly,
                         typeof(context_with_multiple_specifications).GetTypeInfo());
        runner.RunMember(typeof(context_with_duplicate_tags).GetTypeInfo().Assembly,
                         typeof(context_with_duplicate_tags).GetTypeInfo());
      };

    It should_run_everything = () =>
      testListener.SpecCount.ShouldEqual(3);

    static DefaultRunner runner;
    static TestListener testListener;
  }

  [Subject("Specification Runner")]
  public class when_running_with_context_filters
  {
    Establish context = () =>
    {
      testListener = new TestListener();
      var options = new RunOptions(new string[] { }, new string[] { }, new[] { "Example.Random.context_with_multiple_specifications" });

      runner = new DefaultRunner(testListener, options);
    };

    Because of = () =>
      {
        runner.RunMember(typeof(context_with_multiple_specifications).GetTypeInfo().Assembly,
                         typeof(context_with_multiple_specifications).GetTypeInfo());
        runner.RunMember(typeof(context_with_duplicate_tags).GetTypeInfo().Assembly,
                         typeof(context_with_duplicate_tags).GetTypeInfo());
      };

    It should_run_included_contexts_only = () =>
      testListener.SpecCount.ShouldEqual(2);

    static DefaultRunner runner;
    static TestListener testListener;
  }

  [Subject("Specification Runner")]
  public class when_running_with_specification_filters
  {
    Establish context = () =>
    {
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
        runner.RunMember(typeof(context_with_multiple_specifications).GetTypeInfo().Assembly,
                         typeof(context_with_multiple_specifications).GetTypeInfo());
        runner.RunMember(typeof(context_with_duplicate_tags).GetTypeInfo().Assembly,
                         typeof(context_with_duplicate_tags).GetTypeInfo());
      };

    It should_run_included_specifications_only = () =>
      testListener.SpecCount.ShouldEqual(3);

    static DefaultRunner runner;
    static TestListener testListener;
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_public_Its
    : RunnerSpecs
  {
    Because of = Run<context_with_public_It_field>;

    It should_succeed =
      () => testListener.SpecCount.ShouldEqual(1);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_protected_Its
    : RunnerSpecs
  {
    Because of = Run<context_with_protected_It_field>;

    It should_succeed =
      () => testListener.SpecCount.ShouldEqual(1);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_internal_Its
    : RunnerSpecs
  {
    Because of = Run<context_with_internal_It_field>;

    It should_succeed =
      () => testListener.SpecCount.ShouldEqual(1);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_public_Behaves_like
    : RunnerSpecs
  {
    Because of = Run<context_with_public_Behaves_like_field>;

    It should_succeed =
      () => testListener.SpecCount.ShouldEqual(1);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_nonprivate_framework_fields
    : RunnerSpecs
  {
    Because of = Run<context_with_nonprivate_framework_fields>;

    It should_succeed =
      () => testListener.LastResult.Passed.ShouldBeTrue();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_inside_a_static_class
    : RunnerSpecs
  {
    Because of = Run<StaticContainer.when_a_context_is_nested_inside_a_static_class>;

    It should_succeed =
      () => testListener.LastResult.Passed.ShouldBeTrue();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_inside_a_static_class_that_is_nested_in_a_nonstatic_class
    : RunnerSpecs
  {
    Because of = Run<NonStaticContainer.StaticContainer.when_a_context_is_nested_inside_a_static_class_that_is_nested_inside_a_class>;

    It should_succeed =
      () => testListener.LastResult.Passed.ShouldBeTrue();
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
