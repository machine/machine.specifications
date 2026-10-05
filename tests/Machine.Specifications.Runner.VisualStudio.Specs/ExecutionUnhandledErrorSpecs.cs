using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Specs
{
    class ExecutionUnhandledErrorSpecs : WithFakes
    {
        static MspecTestExecutor adapter;

        Establish context = () =>
        {
            The<ISpecificationExecutor>()
                .WhenToldTo(d => d.RunAssemblySpecifications(
                    Param<string>.IsAnything,
                    Param<VisualStudioTestIdentifier[]>.IsAnything,
                    Param<Uri>.IsAnything,
                    Param<IFrameworkHandle>.IsAnything))
                .Throw(new InvalidOperationException());

            var adapterDiscoverer = new MspecTestDiscoverer(An<ISpecificationDiscoverer>());
            adapter = new MspecTestExecutor(The<ISpecificationExecutor>(), adapterDiscoverer, An<ISpecificationFilterProvider>());
        };

        Because of = () =>
            adapter.RunTests(new[] {new TestCase("a", MspecTestRunner.Uri, "dll"), }, An<IRunContext>(), The<IFrameworkHandle>());

        It should_send_an_error_notification_to_visual_studio = () =>
            The<IFrameworkHandle>()
                .WasToldTo(logger => logger.SendMessage(TestMessageLevel.Error, Param<string>.IsNotNull))
                .OnlyOnce();
    }
}
