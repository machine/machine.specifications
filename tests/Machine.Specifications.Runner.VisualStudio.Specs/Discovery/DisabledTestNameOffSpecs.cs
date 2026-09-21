using System;
using System.Reflection;
using Machine.Fakes;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
     class DisabledTestNameOffSpecs : WithFakes
    {
        static Assembly assembly;

        Establish context = () =>
            assembly = typeof(StandardSpec).Assembly;

        Because of = () =>
            The<MspecTestRunner>()
                .DiscoverTests(new[] {assembly.GetType("SampleSpecs.When_something").GetTypeInfo().Assembly.Location},
                    An<IDiscoveryContext>(),
                    An<IMessageLogger>(),
                    The<ITestCaseDiscoverySink>());


        It should_use_full_type_and_field_name_for_display_name = () =>
            The<ITestCaseDiscoverySink>()
                .WasToldTo(d => d.SendTestCase(Param<TestCase>.Matches(t => t.DisplayName.Equals("When something it should pass", StringComparison.Ordinal))))
                .OnlyOnce();
    }
}
