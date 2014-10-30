using System.Reflection;
using Machine.Specifications.Runner.Utility;
using TestDriven.Framework;

namespace Machine.Specifications.TDNetRunner
{
    public class SpecificationRunner : ITestRunner
    {
        public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
        {
            var listener = new TDNetRunListener(testListener);
            var runner = new AppDomainRunner(listener, RunOptions.Default);
            runner.RunAssembly(new AssemblyPath(assembly.Location));

            return listener.TestRunState;
        }

        public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            var listener = new TDNetRunListener(testListener);
            var runner = new AppDomainRunner(listener, RunOptions.Default);
            runner.RunNamespace(new AssemblyPath(assembly.Location), ns);

            return listener.TestRunState;
        }

        public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            var listener = new TDNetRunListener(testListener);
            var runner = new AppDomainRunner(listener, RunOptions.Default);
            runner.RunMember(new AssemblyPath(assembly.Location), member);

            return listener.TestRunState;
        }
    }
}
