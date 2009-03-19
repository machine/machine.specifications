using System;
using System.IO;
using System.Reflection;

using Machine.Specifications.Example;
using Machine.Specifications.Example.BindingFailure;
using Machine.Specifications.Example.CleanupFailure;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Specs.Runner
{
  public class running_specs
  {
    public static TestListener listener;
    public static AppDomainRunner runner;

    Establish context = () =>
    {
      listener = new TestListener();
      runner = new AppDomainRunner(listener, RunOptions.Default);
    };
  }

  public class when_running_specs_by_assembly : running_specs
  {
    Because of = () =>
      runner.RunAssembly(typeof(Account).Assembly);

    It should_run_them_all = () =>
      listener.SpecCount.ShouldEqual(6);
  }

  public class when_running_specs_in_an_assembly_with_a_reference_that_cannot_be_bound : running_specs
  {
    static Exception Exception;
    const string ReferencedAssembly = "Machine.Specifications.Example.BindingFailure.Ref.dll";

    Establish context = () =>
    {
      if (File.Exists(ReferencedAssembly))
      {
        File.Delete(ReferencedAssembly);
      }
    };

    Because of = () =>
      runner.RunAssembly(typeof(if_a_referenced_assembly_cannot_be_bound).Assembly);

    It should_fail = () =>
      listener.LastFatalError.ShouldNotBeNull();
    //Exception.ShouldBeOfType<TargetInvocationException>();
  }

  [Ignore]
  public class when_running_specs_in_which_the_cleanup_throws_a_non_serializable_exception : running_specs
  {
    Because of = () =>
      runner.RunAssembly(typeof(cleanup_failure).Assembly);

    It should_cause_a_fatal_error = () =>
      listener.LastFatalError.ShouldNotBeNull();

  }

  public class when_running_specs_by_namespace : running_specs
  {
    Because of = () =>
      runner.RunNamespace(typeof(Account).Assembly, "Machine.Specifications.Example");

    It should_run_them_all = () =>
      listener.SpecCount.ShouldEqual(6);
  }

  public class when_running_specs_by_member : running_specs
  {
    Because of = () =>
      runner.RunMember(typeof(Account).Assembly, typeof(when_transferring_an_amount_larger_than_the_balance_of_the_from_account).GetField("should_not_allow_the_transfer", BindingFlags.NonPublic | BindingFlags.Instance));

    It should_run = () =>
      listener.SpecCount.ShouldEqual(1);
  }

  public class TestListener : ISpecificationRunListener
  {
    public int SpecCount = 0;
    public Result LastResult;
    public void OnAssemblyStart(AssemblyInfo assembly)
    {
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
    }

    public void OnRunStart()
    {
      LastResult = null;
    }

    public void OnRunEnd()
    {
    }

    public void OnContextStart(ContextInfo context)
    {
    }

    public void OnContextEnd(ContextInfo context)
    {
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      LastResult = result;
      SpecCount++;
    }

    public void OnFatalError(ExceptionResult exception)
    {
      LastFatalError = exception;
    }

    public ExceptionResult LastFatalError { get; private set; }
  }
}