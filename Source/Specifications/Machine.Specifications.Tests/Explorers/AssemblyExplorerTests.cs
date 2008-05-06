using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Example;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Testing;
using NUnit.Framework;

namespace Machine.Specifications.Explorers
{
  [TestFixture]
  public class AssemblyExplorer_FindSpecifications_WithExampleAssemblyTests : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Description> specifications;

    public override void BeforeEachTest()
    {
      specifications = Target.FindDescriptionsIn(typeof(Account).Assembly);
    }

    [Test]
    public void ShouldReturnFourSpecifications()
    {
      specifications.Count().ShouldEqual(3);
    }

    [Test]
    public void ShouldReturnFourSpecificationsNamedCorrectly()
    {
      var names = specifications.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "Transferring between from account and to account",
        "Transferring between two accounts",
        "Transferring an amount greater than the balance of the from account");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindSpecifications_WithOneOfManyNamespaces : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Description> specifications;

    public override void BeforeEachTest()
    {
      specifications = Target.FindDescriptionsIn(typeof(ExampleA.InExampleA_1).Assembly, "Machine.Specifications.ExampleA");
    }

    [Test]
    public void ShouldReturnTwoSpecifications()
    {
      specifications.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldReturnFourSpecificationsNamedCorrectly()
    {
      var names = specifications.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "InExampleA 1",
        "InExampleA 2");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindSpecifications_WithFirstSpecificationOfExampleAssemblyTests : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Description> specifications;
    private Description description;

    public override void  BeforeEachTest()
    {
      specifications = Target.FindDescriptionsIn(typeof (Account).Assembly);
      description =
        specifications.Where(x => x.Name == "Transferring between from account and to account").FirstOrDefault();
      description.ShouldNotBeNull();
    }

    [Test]
    public void ShouldHaveTwoRequirements()
    {
      description.Requirements.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldHaveRequirementsWithCorrectItClauses()
    {
      var names = description.Requirements.Select(x => x.ItClause).ToList();
      names.ShouldContainOnly(
        "should debit the from account by the amount transferred",
        "should credit the to account by the amount transferred");
    }

    [Test]
    public void ShouldHaveRequirementsWithCorrectWhenClauses()
    {
      description.WhenClause.ShouldEqual("the transfer is made");
    }
  }
}
