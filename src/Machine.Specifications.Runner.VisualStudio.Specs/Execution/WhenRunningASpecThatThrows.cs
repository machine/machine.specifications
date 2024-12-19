using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    class WhenRunningASpecThatThrows : WithSingleSpecExecutionSetup
    {
        Establish context = () =>
            specification_to_run = new VisualStudioTestIdentifier("SampleSpecs.StandardSpec", "unhandled_exception");

        It should_tell_visual_studio_it_failed = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(handle =>
                    handle.RecordEnd(
                        Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(specification_to_run)),
                        Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Failed)))
                .OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(handle =>
                    handle.RecordResult(Param<TestResult>.Matches(result => result.Outcome == TestOutcome.Failed)))
                .OnlyOnce();
        };
    }
}
