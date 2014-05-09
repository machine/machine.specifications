using System;

using Example;

using FluentAssertions;

using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Specs.Generation
{
    [Subject(typeof(SpecificationTreeListener))]
    public class when_getting_a_tree_from_a_spec_run
    {
        static ISpecificationRunner runner;
        static AssemblyPath specAssemblyPath;
        static SpecificationTreeListener listener;

        Establish context = () =>
          {
              listener = new SpecificationTreeListener();
              runner = new SpecificationRunner();
              specAssemblyPath = new AssemblyPath(typeof (when_a_customer_first_views_the_account_summary_page).Assembly.Location);
          };

        Because of =
          () => runner.RunAssemblies(new [] { specAssemblyPath }, listener, RunOptions.Default);

        It should_set_the_total_specifications =
          () => listener.Run.TotalSpecifications.Should().Be(6);

        It should_set_the_report_generation_date =
          () => DateTime.Now.AddSeconds(-5).Should().BeOnOrBefore(listener.Run.Meta.GeneratedAt);

        It should_default_to_no_timestamp =
          () => listener.Run.Meta.ShouldGenerateTimeInfo.Should().BeFalse();

        It should_default_to_no_link_to_the_summary =
          () => listener.Run.Meta.ShouldGenerateIndexLink.Should().BeFalse();
    }
}