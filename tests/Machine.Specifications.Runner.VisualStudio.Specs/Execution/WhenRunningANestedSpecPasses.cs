using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    class WhenRunningANestedSpecPasses : WithSingleSpecExecutionSetup
    {
        Establish context = () =>
            specification_to_run = new VisualStudioTestIdentifier("SampleSpecs.Parent+NestedSpec", "should_remember_that_true_is_true");

        It should_tell_visual_studio_it_passed = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(handle =>
                    handle.RecordEnd(
                        Param<TestCase>.Matches(x => x.ToVisualStudioTestIdentifier().Equals(specification_to_run)),
                        Param<TestOutcome>.Matches(x => x == TestOutcome.Passed)))
                .OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordResult(Param<TestResult>.Matches(result => result.Outcome == TestOutcome.Passed)))
                .OnlyOnce();
        };
    }
}
