using System.Linq;
using NUnit.Framework;

using TestStatus = Gallio.Model.TestStatus;

namespace Machine.Specifications.GallioAdapter.Tests
{
  [TestFixture]
  public class BankingSpecs_when_transferring_between_two_accounts_Tests
  {
    Gallio.Runner.Reports.Schema.TestStepRun _run;

    [SetUp]
    public void Setup()
    {
      _run = GallioRunner.RunAllSpecificationsFor<Example.when_transferring_between_two_accounts>();
    }

    [Test]
    public void ShouldPassAllSpecifications()
    {
      _run.AllTestStepRuns.All(x => x.Result.Outcome.Status == TestStatus.Passed).ShouldBeTrue();
    }

    [Test]
    public void ShouldHaveTwoChildren()
    {
      _run.Children.Count().ShouldEqual(2);
    }    
  }

  [TestFixture]
  public class BankingSpecs_when_transferring_an_amount_larger_than_the_balance_of_the_from_account_Tests
  {
    Gallio.Runner.Reports.Schema.TestStepRun _run;

    [SetUp]
    public void Setup()
    {
      _run = GallioRunner.RunAllSpecificationsFor<Example.when_transferring_an_amount_larger_than_the_balance_of_the_from_account>();
    }

    [Test]
    public void ShouldPassAllSpecifications()
    {
      _run.AllTestStepRuns.All(x => x.Result.Outcome.Status == TestStatus.Passed).ShouldBeTrue();
    }

    [Test]
    public void ShouldHaveOneChild()
    {
      _run.Children.Count().ShouldEqual(1);
    }
  }
}
