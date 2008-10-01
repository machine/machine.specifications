using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Runner;
using Machine.Specifications.Specs.Runner;

namespace Machine.Specifications.Specs.Runner
{
  [Subject("Specification Runner")]
  public class when_running_a_context_with_no_specifications
    : with_runner
  {
    Establish context = () =>
    {
      context_with_no_specs.ContextEstablished = false;
      context_with_no_specs.CleanupOccurred = false;
    };

    Because of =()=>
      Run<context_with_no_specs>();

    It should_not_establish_the_context =()=>
      context_with_no_specs.ContextEstablished.ShouldBeFalse();

    It should_not_cleanup =()=>
      context_with_no_specs.CleanupOccurred.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_an_ignored_specifications
    : with_runner
  {
    Establish context = () =>
    {
      context_with_ignore_on_one_spec.IgnoredSpecRan = false;
    };

    Because of = () =>
      Run<context_with_ignore_on_one_spec>();

    It should_not_run_the_spec =()=>
      context_with_ignore_on_one_spec.IgnoredSpecRan.ShouldBeFalse();

    It should_not_establish_the_context =()=>
      context_with_ignore_on_one_spec.ContextEstablished.ShouldBeFalse();

    It should_not_cleanup =()=>
      context_with_ignore_on_one_spec.CleanupOccurred.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_an_ignored_context
    : with_runner
  {
    Establish context = () =>
    {
      context_with_ignore.IgnoredSpecRan = false;
    };

    Because of = () =>
      Run<context_with_ignore>();

    It should_not_run_the_spec =()=>
      context_with_ignore.IgnoredSpecRan.ShouldBeFalse();

    It should_not_establish_the_context =()=>
      context_with_ignore.ContextEstablished.ShouldBeFalse();

    It should_not_cleanup =()=>
      context_with_ignore.CleanupOccurred.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_specifications
    : with_runner
  {
    Establish context = () =>
    {
      context_with_multiple_specifications.EstablishRunCount = 0;
      context_with_multiple_specifications.BecauseClauseRunCount = 0;
    };

    Because of = () =>
      Run<context_with_multiple_specifications>();

    It should_establish_the_context_once =()=>
      context_with_multiple_specifications.EstablishRunCount.ShouldEqual(1);

    It should_invoke_the_because_clause_once =()=>
      context_with_multiple_specifications.BecauseClauseRunCount.ShouldEqual(1);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_specifications_and_setup_once_per_attribute
    : with_runner 
  {

    Establish context = () =>
    {
      context_with_multiple_specifications_and_setup_for_each.EstablishRunCount = 0;
      context_with_multiple_specifications_and_setup_for_each.BecauseClauseRunCount = 0;
    };

    Because of =()=>
      Run<context_with_multiple_specifications_and_setup_for_each>();

    It should_establish_the_context_once =()=>
      context_with_multiple_specifications_and_setup_for_each.EstablishRunCount.ShouldEqual(2);

    It should_invoke_the_because_clause_once =()=>
      context_with_multiple_specifications_and_setup_for_each.BecauseClauseRunCount.ShouldEqual(2);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_multiple_establish_clauses
    : with_runner
  {
    static Exception exception;

    Because foo;
    Because of =()=>
      exception = Catch.Exception(Run<context_with_multiple_establish_clauses>);

    It should_fail =()=>
      exception.ShouldBeOfType<SpecificationUsageException>();
  }

  public class with_runner
  {
    static SpecificationRunner runner;

    Establish context =()=>
    {
      runner = new SpecificationRunner(new TestListener(), RunOptions.Default);
    };

    public static void Run<T>()
    {
      runner.RunMember(typeof(T).Assembly, typeof(T));
    }
  }
}