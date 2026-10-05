using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    class WhenRunningAnAssemblyWithBehaviors : WithAssemblyExecutionSetup
    {
        static VisualStudioTestIdentifier specification_expected_to_run;

        Establish context = () =>
            specification_expected_to_run = new VisualStudioTestIdentifier("SampleSpecs.BehaviorSampleSpec", "sample_behavior_test");

        It should_run_all_behaviors = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(handle => 
                    handle.RecordEnd(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(specification_expected_to_run)),
                                     Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed)))
                .OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(handle =>
                    handle.RecordResult(Param<TestResult>.Matches(result =>
                        result.Outcome == TestOutcome.Passed &&
                        result.TestCase.ToVisualStudioTestIdentifier().Equals(specification_expected_to_run))))
                .OnlyOnce();
        };
    }
}
