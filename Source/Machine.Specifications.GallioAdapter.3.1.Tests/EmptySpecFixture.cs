using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using NUnit.Framework;
using Gallio.Common.Markup;

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
            _run.Result.Outcome.Status.ShouldEqual(Gallio.Model.TestStatus.Failed);
        }

        [Test]
        public void ShouldHaveFailedAllThreeChildren()
        {
            _run.Children.Count().ShouldEqual(3);
            _run.Children.All(c => c.Result.Outcome.Status == Gallio.Model.TestStatus.Failed).ShouldBeTrue();
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
            //child.Step.Name.ShouldEqual(expected);            
            var log = child.TestLog.GetStream(MarkupStreamNames.Failures).ToString();
            log.Contains( expected ).ShouldBeTrue();            
        }
    }
}
