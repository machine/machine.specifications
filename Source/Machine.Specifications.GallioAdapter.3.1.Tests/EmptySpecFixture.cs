using System.Linq;
using Gallio.Common.Markup;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
  [TestFixture]
  public class EmptySpecFixture
  {
    Gallio.Runner.Reports.Schema.TestStepRun _testStepRun;

    [SetUp]
    public void Setup()
    {
      _testStepRun = GallioRunner.RunAllSpecsFor<Example.when_a_customer_first_views_the_account_summary_page>();      
    }

    [Test]
    public void ShouldHaveFailedTheTest()
    {
      _testStepRun.Result.Outcome.ShouldEqual( Gallio.Model.TestOutcome.Pending);
    }

    [Test]
    public void ShouldHaveFailedAllThreeChildren()
    {
      _testStepRun.Children.Count().ShouldEqual(3);
      _testStepRun.Children.All(c => c.Result.Outcome == Gallio.Model.TestOutcome.Pending).ShouldBeTrue();
    }

    [Test]
    public void ShouldHaveIndicatThat_should_display_all_account_transactions_for_the_past_thirty_days_IsNotImplemented()
    {       
      CheckNotImplementedFor("should display all account transactions for the past thirty days"); 
    }

    [Test]
    public void ShouldHaveIndicatThat_should_display_debit_amounts_in_red_text_IsNotImplemented()
    {
      CheckNotImplementedFor("should display debit amounts in red text");
    }

    [Test]
    public void ShouldHaveIndicatThat_should_display_deposit_amounts_in_black_text_IsNotImplemented()
    {
      CheckNotImplementedFor("should display deposit amounts in black text");
    }

    void CheckNotImplementedFor(string spec)
    {
      var expected = string.Format("{0} (Not Implemented)", spec);
      var child = _testStepRun.Children.Single(x => x.Step.Name == spec);
            
      var log = child.TestLog.GetStream(MarkupStreamNames.Failures).ToString();
      log.Contains( expected ).ShouldBeTrue();

      child.Result.Outcome.ShouldEqual( Gallio.Model.TestOutcome.Pending);
      _testStepRun.Result.Outcome.ShouldEqual(Gallio.Model.TestOutcome.Pending);
    }
  }
}
