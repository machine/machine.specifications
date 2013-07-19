using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Explorers;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.TDNetRunner;
using TestDriven.Framework;

namespace Machine.Specifications.TDNetRunner
{
  public class SpecificationRunner : ITestRunner
  {
    public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new AppDomainRunner(listener, RunOptions.Default);
      try
      {
        runner.StartRun(assembly);
        runner.RunAssembly(assembly);
      }
      finally
      {
        runner.EndRun(assembly);
      }

      return listener.TestRunState;
    }

    public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new AppDomainRunner(listener, RunOptions.Default);
      try
      {
        runner.StartRun(assembly);
        runner.RunNamespace(assembly, ns);
      }
      finally
      {
        runner.EndRun(assembly);
      }

      return listener.TestRunState;
    }

    public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new AppDomainRunner(listener, RunOptions.Default);
      try
      {
        runner.StartRun(assembly);
        runner.RunMember(assembly, member);
      }
      finally
      {
        runner.EndRun(assembly);
      }

      return listener.TestRunState;
    }
  }
}
