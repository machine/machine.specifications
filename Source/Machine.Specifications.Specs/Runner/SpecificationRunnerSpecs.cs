using System;
using Machine.Specifications.Example;
using Machine.Specifications.FailingExample;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Specs.Runner
{
  [Subject("Specification Runner")]
  public class when_running_a_context_with_no_specifications
    : with_runner
  {
    Establish context = () =>
    {
      TestCleanupAfterEveryContext.AfterContextCleanupRun = false;
      context_with_no_specs.ContextEstablished = false;
      context_with_no_specs.CleanupOccurred = false;
    };

    Because of = () =>
      Run<context_with_no_specs>();

    It should_not_establish_the_context = () =>
      context_with_no_specs.ContextEstablished.ShouldBeFalse();

    It should_not_cleanup = () =>
      context_with_no_specs.CleanupOccurred.ShouldBeFalse();

    It should_not_perform_assembly_wide_cleanup = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_an_ignored_specifications
    : with_runner
  {
    Establish context = () =>
    {
      context_with_no_specs.ContextEstablished = false;
      context_with_no_specs.CleanupOccurred = false;
      context_with_ignore_on_one_spec.IgnoredSpecRan = false;
      TestCleanupAfterEveryContext.AfterContextCleanupRun = false;
    };

    Because of = () =>
      Run<context_with_ignore_on_one_spec>();

    It should_not_run_the_spec = () =>
      context_with_ignore_on_one_spec.IgnoredSpecRan.ShouldBeFalse();

    It should_not_establish_the_context = () =>
      context_with_ignore_on_one_spec.ContextEstablished.ShouldBeFalse();

    It should_not_cleanup = () =>
      context_with_ignore_on_one_spec.CleanupOccurred.ShouldBeFalse();

    It should_not_perform_assembly_wide_cleanup = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_an_ignored_context
    : with_runner
  {
    Establish context = () =>
    {
      context_with_ignore.IgnoredSpecRan = false;
      TestCleanupAfterEveryContext.AfterContextCleanupRun = false;
    };

    Because of = () =>
      Run<context_with_ignore>();

    It should_not_run_the_spec = () =>
      context_with_ignore.IgnoredSpecRan.ShouldBeFalse();

    It should_not_establish_the_context = () =>
      context_with_ignore.ContextEstablished.ShouldBeFalse();

    It should_not_cleanup = () =>
      context_with_ignore.CleanupOccurred.ShouldBeFalse();

    It should_not_perform_assembly_wide_cleanup = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_specifications
    : with_runner
  {
    Establish context = () =>
    {
      context_with_multiple_specifications.EstablishRunCount = 0;
      context_with_multiple_specifications.BecauseClauseRunCount = 0;
      TestCleanupAfterEveryContext.Reset();
    };

    Because of = () =>
      Run<context_with_multiple_specifications>();

    It should_establish_the_context_once = () =>
      context_with_multiple_specifications.EstablishRunCount.ShouldEqual(1);

    It should_invoke_the_because_clause_once = () =>
      context_with_multiple_specifications.BecauseClauseRunCount.ShouldEqual(1);

    It should_invoke_the_assembly_wide_cleanup_once = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRunCount.ShouldEqual(1);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_specifications_and_setup_once_per_attribute
    : with_runner
  {
    Establish context = () =>
    {
      context_with_multiple_specifications_and_setup_for_each.EstablishRunCount = 0;
      context_with_multiple_specifications_and_setup_for_each.BecauseClauseRunCount = 0;
      TestCleanupAfterEveryContext.Reset();
    };

    Because of = () =>
      Run<context_with_multiple_specifications_and_setup_for_each>();

    It should_establish_the_context_for_each_specification = () =>
      context_with_multiple_specifications_and_setup_for_each.EstablishRunCount.ShouldEqual(2);

    It should_invoke_the_because_clause_for_each_specification = () =>
      context_with_multiple_specifications_and_setup_for_each.BecauseClauseRunCount.ShouldEqual(2);

    It should_invoke_the_assembly_wide_cleanup_once_per_spec = () =>
      TestCleanupAfterEveryContext.AfterContextCleanupRunCount.ShouldEqual(2);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_establish_clauses
    : with_runner
  {
    static Exception exception;

    Because of = () =>
      exception = Catch.Exception(Run<context_with_multiple_establish_clauses>);

    It should_fail = () =>
      exception.ShouldBeOfType<SpecificationUsageException>();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_failing_establish_clauses
    : with_runner
  {
    Because of = Run<context_with_failing_establish>;

    It should_fail = () =>
    testListener.LastResult.Passed.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_failing_because_clauses
    : with_runner
  {
    Because of = Run<context_with_failing_because>;

    It should_fail = () =>
    testListener.LastResult.Passed.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_failing_specs
  : with_runner
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
      runner = new DefaultRunner(new TestListener(), new RunOptions(new[] { "asdfasdf" }, new string[0]));
    };

    Because of = () =>
      runner.RunAssembly(typeof(TestAssemblyContext).Assembly);

    It should_not_run_assembly_start = () =>
      TestAssemblyContext.OnAssemblyStartRun.ShouldBeFalse();

    It should_not_run_assembly_complete = () =>
      TestAssemblyContext.OnAssemblyCompleteRun.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  [Ignore]
  public class when_running_a_specification_with_console_output
    : with_runner
  {
    Because of = () =>
      Run<context_with_console_output>();

    It should_capture_the_console_out_stream = () =>
      testListener.LastResult.ConsoleOut.ShouldEqual("Console.Out message in establish\r\n" +
        "Console.Out message in because\r\n" +
        "Console.Out message in spec\r\n");

    It should_capture_the_console_error_stream = () =>
      testListener.LastResult.ConsoleError.ShouldEqual("Console.Error message in establish\r\n" +
        "Console.Error message in because\r\n" +
        "Console.Error message in spec\r\n");
  }

  [Subject("Specification Runner")]
  [Ignore]
  public class when_running_a_specification_with_console_output_and_foreach
    : with_runner
  {
    Because of = () =>
      Run<context_with_console_output_and_for_each>();

    It should_capture_the_console_out_stream = () =>
      testListener.LastResult.ConsoleOut.ShouldEqual("Console.Out message in establish\r\n" +
        "Console.Out message in because\r\n" +
        "Console.Out message in spec\r\n" +
        "Console.Out message in cleanup\r\n");

    It should_capture_the_console_error_stream = () =>
      testListener.LastResult.ConsoleError.ShouldEqual("Console.Error message in establish\r\n" +
        "Console.Error message in because\r\n" +
        "Console.Error message in spec\r\n" +
        "Console.Error message in cleanup\r\n");
  }

  [Subject("Specification Runner")]
  public class when_running_a_specification_that_throws_an_exception_with_an_inner_exception
    : with_runner
  {
    Because of =()=>
      Run<context_with_inner_exception>();

    It should_include_the_inner_exception_in_the_result =()=>
      testListener.LastResult.Exception.ToString().ShouldContain("INNER123");
  }

  [Subject("Specification Runner")]
  public class when_running_a_behavior
    : with_runner
  {
    Establish context = () =>
    {
      Behaviors.BehaviorSpecRan = false;
    };

    Because of = () =>
      Run<Behaviors>();

    It should_not_run_the_behavior_specs = () =>
      Behaviors.BehaviorSpecRan.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_with_tag
  {
    Establish context = () =>
    {
      TaggedCleanup.Reset();
      UntaggedCleanup.Reset();
      UntaggedAssemblyContext.Reset();
      TaggedAssemblyContext.Reset();

      testListener = new TestListener();
      var options = new RunOptions(new string[] {"foobar"}, new string[] {});

      runner = new DefaultRunner(testListener, options);
    };

    Because of = () =>
      runner.RunMember(typeof(context_with_multiple_specifications).Assembly,
                       typeof(context_with_multiple_specifications));

    It should_not_run_untagged_assembly_context = () =>
      UntaggedAssemblyContext.OnAssemblyStartRun.ShouldBeFalse();

    It should_run_tagged_assembly_context = () =>
      TaggedAssemblyContext.OnAssemblyStartRun.ShouldBeTrue();

    It should_not_run_untagged_assembly_context_complete = () =>
      UntaggedAssemblyContext.OnAssemblyCompleteRun.ShouldBeFalse();

    It should_run_tagged_assembly_context_complete = () =>
      TaggedAssemblyContext.OnAssemblyCompleteRun.ShouldBeTrue();

    It should_not_run_untagged_global_cleanup = () =>
      UntaggedCleanup.AfterContextCleanupRunCount.ShouldEqual(0);

    It should_run_tagged_global_cleanup = () =>
      TaggedCleanup.AfterContextCleanupRunCount.ShouldBeGreaterThan(0);

    static DefaultRunner runner;
    static TestListener testListener;
  }

  public class with_runner
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
      runner.RunMember(typeof(T).Assembly, typeof(T));
    }
  }
}
