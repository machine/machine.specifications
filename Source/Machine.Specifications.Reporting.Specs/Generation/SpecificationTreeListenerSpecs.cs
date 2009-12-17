using System;

using Machine.Specifications.Example;
using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Reporting.Specs.Generation
{
  [Subject(typeof(SpecificationTreeListener))]
  public class when_getting_a_tree_from_a_spec_run
  {
    static DefaultRunner runner;
    static SpecificationTreeListener listener;

    Establish context = () =>
      {
        listener = new SpecificationTreeListener();
        runner = new DefaultRunner(listener, RunOptions.Default);
      };

    Because of =
      () => runner.RunAssembly(typeof(when_a_customer_first_views_the_account_summary_page).Assembly);

    It should_set_the_total_specifications =
      () => listener.Run.TotalSpecifications.ShouldEqual(6);

    It should_set_the_report_generation_date =
      () => DateTime.Now.AddSeconds(-5).ShouldBeLessThan(listener.Run.Meta.GeneratedAt);
    
    It should_default_to_no_timestamp =
      () => listener.Run.Meta.ShouldGenerateTimeInfo.ShouldBeFalse();
  }
}


