using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    class WhenRunningASingleBehaviorPasses : WithSingleSpecExecutionSetup
    {
        Establish context = () => 
            specification_to_run = new VisualStudioTestIdentifier("SampleSpecs.BehaviorSampleSpec", "sample_behavior_test");

        It should_tell_visual_studio_it_passed = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(handle => 
                    handle.RecordEnd(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(specification_to_run)),
                                     Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed)))
                .OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(handle => 
                    handle.RecordResult(Param<TestResult>.Matches(result => result.Outcome == TestOutcome.Passed)))
                .OnlyOnce();
        };
    }
}
