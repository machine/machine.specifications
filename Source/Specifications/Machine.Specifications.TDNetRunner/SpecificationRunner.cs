using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Explorers;
using Machine.Specifications.Runner;
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
      runner.RunAssembly(assembly);

      return listener.TestRunState;
    }

    public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new AppDomainRunner(listener, RunOptions.Default);
      runner.RunNamespace(assembly, ns);

      return listener.TestRunState;
    }

    public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new AppDomainRunner(listener, RunOptions.Default);
      runner.RunMember(assembly, member);

      return listener.TestRunState;
    }
  }
}
