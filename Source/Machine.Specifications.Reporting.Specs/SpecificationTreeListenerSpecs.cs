using Machine.Specifications.Example;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Reporting.Specs.SpecificationTreeListenerSpecs
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

    Because of = () =>
      runner.RunAssembly(typeof(when_a_customer_first_views_the_account_summary_page).Assembly);

    It should_set_the_total_specifications_ = () =>
      listener.Run.TotalSpecifications.ShouldEqual(6);
  }
}
