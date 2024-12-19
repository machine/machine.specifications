using System.Reflection;
using Machine.Fakes;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public abstract class WithAssemblyExecutionSetup : WithFakes
    {
        static MspecTestRunner executor;

        static Assembly assembly;

        Establish context = () =>
        {
            executor = new MspecTestRunner();

            assembly = typeof(StandardSpec).Assembly;
        };

        Because of = () =>
            executor.RunTests(new[] { assembly.Location }, An<IRunContext>(), The<IFrameworkHandle>());
    }
}
