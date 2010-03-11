using System.Linq;
using Gallio.Common.Markup;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
    [TestFixture]
    public class EmptySpecFixture
    {
        Gallio.Runner.Reports.Schema.TestStepRun _run;

        [SetUp]
        public void Setup()
        {
            _run = GallioRunner.RunAllSpecsFor<Example.when_a_customer_first_views_the_account_summary_page>();            
        }

        [Test]
        public void ShouldHaveFailedTheTest()
        {
            _run.Result.Outcome.ShouldEqual( Gallio.Model.TestOutcome.Pending);
        }

        [Test]
        public void ShouldHaveFailedAllThreeChildren()
        {
            _run.Children.Count().ShouldEqual(3);
            _run.Children.All(c => c.Result.Outcome == Gallio.Model.TestOutcome.Pending).ShouldBeTrue();
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
            var child = _run.Children.Single(x => x.Step.Name == spec);
                        
            var log = child.TestLog.GetStream(MarkupStreamNames.Failures).ToString();
            log.Contains( expected ).ShouldBeTrue();

            child.Result.Outcome.ShouldEqual( Gallio.Model.TestOutcome.Pending);
            _run.Result.Outcome.ShouldEqual(Gallio.Model.TestOutcome.Pending);
        }
    }
}
