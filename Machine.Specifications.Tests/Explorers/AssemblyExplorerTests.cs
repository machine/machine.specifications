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
    private IEnumerable<Specification> specifications;

    public override void BeforeEachTest()
    {
      specifications = Target.FindSpecificationsIn(typeof (Account).Assembly);
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
  public class AssemblyExplorer_FindSpecifications_WithFirstSpecificationOfExampleAssemblyTests : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Specification> specifications;
    private Specification specification;

    public override void  BeforeEachTest()
    {
      specifications = Target.FindSpecificationsIn(typeof (Account).Assembly);
      specification =
        specifications.Where(x => x.Name == "Transferring between from account and to account").FirstOrDefault();
      specification.ShouldNotBeNull();
    }

    [Test]
    public void ShouldHaveTwoRequirements()
    {
      specification.Requirements.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldHaveRequirementsWithCorrectItClauses()
    {
      var names = specification.Requirements.Select(x => x.ItClause).ToList();
      names.ShouldContainOnly(
        "should debit the from account by the amount transferred",
        "should credit the to account by the amount transferred");
    }

    [Test]
    public void ShouldHaveRequirementsWithCorrectWhenClauses()
    {
      specification.WhenClause.ShouldEqual("the transfer is made");
    }
  }
}
