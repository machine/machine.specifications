using System;
using System.IO;
using System.Reflection;

using Example;
using Example.CleanupFailure;
using Example.Random;

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
    readonly static string SpecAssembly = GetPath("Example.BindingFailure.dll");
    readonly static string ReferencedAssembly = GetPath("Example.BindingFailure.Ref.dll");

    Establish context = () =>
    {
      if (File.Exists(ReferencedAssembly))
      {
        File.Delete(ReferencedAssembly);
      }
    };

    Because of = () =>
    runner.RunAssembly(Assembly.LoadFrom(SpecAssembly));

    It should_fail = () =>
      listener.LastFatalError.ShouldNotBeNull();
    //Exception.ShouldBeOfType<TargetInvocationException>();

    static string GetPath(string path)
    {
      return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
    }

  }

  [Ignore("exceptions during cleanup are ignored")]
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
      runner.RunNamespace(typeof(Account).Assembly, "Example");

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

  public class when_running_a_nested_context_by_member : running_specs
  {
    Because of = () =>
      runner.RunMember(typeof(Container).Assembly, typeof(Container.nested_context));

    It should_run = () =>
      listener.SpecCount.ShouldEqual(1);
  }

  public class when_running_specs_of_a_nested_context_by_member : running_specs
  {
    Because of = () =>
      runner.RunMember(typeof(Container).Assembly, typeof(Container.nested_context).GetField("should_be_run", BindingFlags.NonPublic | BindingFlags.Instance));

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