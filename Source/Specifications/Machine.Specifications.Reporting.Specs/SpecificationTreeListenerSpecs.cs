using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Example;
using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.Reporting.Specs.SpecificationTreeListenerSpecs
{
  [Subject(typeof(SpecificationTreeListener))]
  public class when_getting_a_tree_from_a_spec_run
  {
    static SpecificationRunner runner;
    static SpecificationTreeListener listener;

    Establish context = () =>
    {
      listener = new SpecificationTreeListener();
      runner = new SpecificationRunner(listener);
    };

    Because of = () =>
      runner.RunAssembly(typeof(when_a_customer_first_views_the_account_summary_page).Assembly, RunOptions.Default);

    It should_set_the_total_specifications_ = () =>
      listener.Run.TotalSpecifications.ShouldEqual(6);
  }
}
