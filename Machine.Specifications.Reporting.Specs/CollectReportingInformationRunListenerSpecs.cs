using Example.Random;

using FluentAssertions;

using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Runner.Utility;
using RunOptions = Machine.Specifications.Runner.Utility.RunOptions;

namespace Machine.Specifications.Reporting.Specs
{
    [Subject(typeof(CollectReportingInformationRunListener))]
    public class when_running_two_contexts_that_use_the_same_behavior
    {
        static ISpecificationRunner runner;
        static CollectReportingInformationRunListener reportListener;
        static RunOptions runOptions;
        static AssemblyPath specAssemblyPath;

        Establish context = () =>
          {
              reportListener = new CollectReportingInformationRunListener();
              runOptions = RunOptions.Custom.Include(new[] { "behavior usage" });
              specAssemblyPath = new AssemblyPath(typeof (context_with_behaviors).Assembly.Location);

              runner = new AppDomainRunner(reportListener, runOptions);
          };

        Because of = () => runner.RunAssemblies(new[] { specAssemblyPath });

        It should_collect_behavior_specifications_and_context_specifications =
          () => reportListener.ResultsBySpecification.Count.Should().Be(3);
    }
}