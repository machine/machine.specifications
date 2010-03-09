using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Runner.Reports.Schema;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
    [TestFixture]
    public class BankingSpecs_when_transferring_between_two_accounts_Fixture
    {
        TestStepRun _run;

        [SetUp]
        public void Setup()
        {
            _run = GallioRunner.RunAllSpecsFor<Example.when_transferring_between_two_accounts>();
        }

        [Test]
        public void SpecAndChildrenShouldSuceed()
        {
            _run.AllTestStepRuns.All(x => x.Result.Outcome.Status == Gallio.Model.TestStatus.Passed);
        }

        [Test]
        public void ShouldHaveTwoChildren()
        {
            _run.Children.Count().ShouldEqual(2);
        }
    }

    [TestFixture]
    public class BankingSpecs_when_transferring_an_amount_larger_than_the_balance_of_the_from_account_Fixture
    {
        TestStepRun _run;

        [SetUp]
        public void Setup()
        {
            _run = GallioRunner.RunAllSpecsFor<Example.when_transferring_an_amount_larger_than_the_balance_of_the_from_account>();
        }

        [Test]
        public void SpecAndChildrenShouldSuceed()
        {
            _run.AllTestStepRuns.All(x => x.Result.Outcome.Status == Gallio.Model.TestStatus.Passed);
        }

        [Test]
        public void ShouldHaveOneChild()
        {
            _run.Children.Count().ShouldEqual(1);
        }
    }
}
