using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Specs;

namespace Machine.Specifications.Reporting.Specs
{
  [Subject(typeof(CollectReportingInformationRunListener))]
  public class when_running_two_contexts_that_use_the_same_behavior
  {
    static DefaultRunner runner;
    static CollectReportingInformationRunListener reportListener;

    Establish context = () =>
      {
        reportListener = new CollectReportingInformationRunListener();

        runner = new DefaultRunner(reportListener,
                                   new RunOptions(new[] { "behavior usage" }, new string[0], new string[0]));
      };

    Because of = () => runner.RunAssembly(typeof(context_with_behaviors).Assembly);

    It should_collect_behavior_specifications_and_context_specifications =
      () => reportListener.ResultsBySpecification.Count.ShouldEqual(3);
  }
}