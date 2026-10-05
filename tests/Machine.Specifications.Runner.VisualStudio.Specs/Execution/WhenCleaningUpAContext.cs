using System.Linq;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    class WhenCleaningUpAContext : WithMultipleSpecExecutionSetup
    {
        Establish context = () =>
            specifications_to_run = new[]
            {
                new VisualStudioTestIdentifier("SampleSpecs.CleanupSpec", "should_not_increment_cleanup"),
                new VisualStudioTestIdentifier("SampleSpecs.CleanupSpec", "should_have_no_cleanups")
            };

        It should_tell_visual_studio_it_passed = () => 
        {
            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordEnd(
                    Param<TestCase>.Matches(y => specifications_to_run.Contains(y.ToVisualStudioTestIdentifier())), 
                    Param<TestOutcome>.Matches(y => y == TestOutcome.Passed)))
                .Twice();

            The<IFrameworkHandle>()
                .WasToldTo(x =>x.RecordResult(Param<TestResult>.Matches(y => y.Outcome == TestOutcome.Passed)))
                .Twice();
        };
    }
}
