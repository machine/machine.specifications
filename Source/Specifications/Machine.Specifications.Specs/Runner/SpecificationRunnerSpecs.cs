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
  {
    static SpecificationRunner runner;

    Establish context = () =>
    {
      context_with_no_specs.ContextEstablished = false;
      context_with_no_specs.OneTimeContextEstablished = false;
      context_with_no_specs.CleanupOnceOccurred = false;
      context_with_no_specs.CleanupOccurred = false;

      runner = new SpecificationRunner(new TestListener(), RunOptions.Default);
    };

    Because of =()=>
      runner.RunMember(typeof(context_with_no_specs).Assembly, typeof(context_with_no_specs));

    It should_not_establish_the_context =()=>
      context_with_no_specs.ContextEstablished.ShouldBeFalse();

    It should_not_establish_the_one_time_context =()=>
      context_with_no_specs.OneTimeContextEstablished.ShouldBeFalse();

    It should_not_cleanup =()=>
      context_with_no_specs.CleanupOccurred.ShouldBeFalse();

    It should_not_perform_one_time_cleanup =()=>
      context_with_no_specs.CleanupOnceOccurred.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_an_ignored_specifications
  {
    static SpecificationRunner runner;

    Establish context = () =>
    {
      context_with_ignore_on_one_spec.IgnoredSpecRan = false;

      runner = new SpecificationRunner(new TestListener(), RunOptions.Default);
    };

    Because of =()=>
      runner.RunMember(typeof(context_with_ignore_on_one_spec).Assembly, typeof(context_with_ignore_on_one_spec));

    It should_not_run_the_spec =()=>
      context_with_ignore_on_one_spec.IgnoredSpecRan.ShouldBeFalse();

    It should_not_establish_the_context =()=>
      context_with_ignore_on_one_spec.ContextEstablished.ShouldBeFalse();

    It should_not_establish_the_one_time_context =()=>
      context_with_ignore_on_one_spec.OneTimeContextEstablished.ShouldBeFalse();

    It should_not_cleanup =()=>
      context_with_ignore_on_one_spec.CleanupOccurred.ShouldBeFalse();

    It should_not_perform_one_time_cleanup =()=>
      context_with_ignore_on_one_spec.CleanupOnceOccurred.ShouldBeFalse();
  }

  [Subject("Specification Runner")]
  public class when_running_an_ignored_context
  {
    static SpecificationRunner runner;

    Establish context = () =>
    {
      context_with_ignore.IgnoredSpecRan = false;

      runner = new SpecificationRunner(new TestListener(), RunOptions.Default);
    };

    Because of =()=>
      runner.RunMember(typeof(context_with_ignore).Assembly, typeof(context_with_ignore));

    It should_not_run_the_spec =()=>
      context_with_ignore.IgnoredSpecRan.ShouldBeFalse();

    It should_not_establish_the_context =()=>
      context_with_ignore.ContextEstablished.ShouldBeFalse();

    It should_not_establish_the_one_time_context =()=>
      context_with_ignore.OneTimeContextEstablished.ShouldBeFalse();

    It should_not_cleanup =()=>
      context_with_ignore.CleanupOccurred.ShouldBeFalse();

    It should_not_perform_one_time_cleanup =()=>
      context_with_ignore.CleanupOnceOccurred.ShouldBeFalse();
  }
}