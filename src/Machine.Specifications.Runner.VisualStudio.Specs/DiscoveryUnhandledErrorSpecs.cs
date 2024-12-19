using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Specs
{
    class DiscoveryUnhandledErrorSpecs : WithFakes
    {
        static MspecTestRunner runner;

        Establish context = () =>
        {
            The<ISpecificationDiscoverer>()
                .WhenToldTo(d => d.DiscoverSpecs(Param<string>.IsAnything))
                .Return(() => throw new InvalidOperationException());

            runner = new MspecTestRunner(The<ISpecificationDiscoverer>(), An<ISpecificationExecutor>(), An<ISpecificationFilterProvider>());
        };

        Because of = () =>
            runner.DiscoverTests(new[] { "bla" }, An<IDiscoveryContext>(), The<IMessageLogger>(), An<ITestCaseDiscoverySink>());

        It should_send_an_error_notification_to_visual_studio = () =>
            The<IMessageLogger>()
                .WasToldTo(logger => logger.SendMessage(TestMessageLevel.Error, Param<string>.IsNotNull))
                .OnlyOnce();
    }
}
