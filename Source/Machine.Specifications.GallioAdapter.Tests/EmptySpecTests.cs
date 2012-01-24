using System.Linq;
using Gallio.Common.Markup;
using Gallio.Model;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
  [TestFixture]
  public class EmptySpecTests
  {
    Gallio.Runner.Reports.Schema.TestStepRun _run;

    [SetUp]
    public void Setup()
    {      
      _run = GallioRunner.RunAllSpecificationsFor<Example.when_a_customer_first_views_the_account_summary_page>();
    }

    [Test]
    public void ShouldHaveLeftTestPending()
    {
      _run.Result.Outcome.ShouldEqual( TestOutcome.Pending );
    }

    [Test]
    public void ShouldHaveLeftAllThreeChildrenPending()
    {
      _run.Children.Count().ShouldEqual(3);
      _run.Children.All(c => c.Result.Outcome == TestOutcome.Pending).ShouldBeTrue();
    }

    [Test]
    public void ShouldIndicateThat_should_display_all_account_transactions_for_the_past_thirty_days_IsNotImplemented()
    {       
      AssertNotImplementedFor("should display all account transactions for the past thirty days"); 
    }

    [Test]
    public void ShouldIndicateThat_should_display_debit_amounts_in_red_text_IsNotImplemented()
    {
      AssertNotImplementedFor("should display debit amounts in red text");
    }

    [Test]
    public void ShouldIndicateThat_should_display_deposit_amounts_in_black_text_IsNotImplemented()
    {
      AssertNotImplementedFor("should display deposit amounts in black text");
    }

    void AssertNotImplementedFor(string spec)
    {
      var expected = string.Format("{0} (NOT IMPLEMENTED)", spec);
      var child = _run.Children.Single(x => x.Step.Name == spec);
            
      var log = child.TestLog.GetStream(MarkupStreamNames.Warnings).ToString();
      log.Contains( expected ).ShouldBeTrue();

      child.Result.Outcome.ShouldEqual(TestOutcome.Pending);
      _run.Result.Outcome.ShouldEqual(TestOutcome.Pending);
    }
  }
}
