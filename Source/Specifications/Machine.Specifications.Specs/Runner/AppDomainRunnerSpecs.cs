using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Example;
using Machine.Specifications.Runner;

namespace Machine.Specifications.Specs.Runner
{
  public class running_specs
  {
    public static TestListener listener;
    public static AppDomainRunner runner;

    Establish context = () =>
    {
      listener = new TestListener();
      runner = new AppDomainRunner(listener);
    };
  }

  public class when_running_specs_by_assembly : running_specs
  {
    Because of = () => 
      runner.RunAssembly(typeof(Account).Assembly);

    It should_run_them_all = () => 
      listener.SpecCount.ShouldEqual(6);
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
    public void OnAssemblyStart(AssemblyInfo assembly)
    {
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
    }

    public void OnRunStart()
    {
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
      SpecCount++;
    }
  }
}