using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public void ShouldReturnOneDescription()
    {
      specifications.Count().ShouldEqual(1);
    }

    [Test]
    public void ShouldReturnFourDescriptionsNamedCorrectly()
    {
      var names = specifications.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "Transferring between accounts"
        );
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindDescriptions_WithOneOfManyNamespaces : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Description> descriptions;

    public override void BeforeEachTest()
    {
      descriptions = Target.FindDescriptionsIn(typeof(ExampleA.InExampleA_1).Assembly, "Machine.Specifications.ExampleA");
    }

    [Test]
    public void ShouldReturnTwoDescriptions()
    {
      descriptions.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldReturnTwoDescriptionsNamedCorrectly()
    {
      var names = descriptions.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "InExampleA 1",
        "InExampleA 2");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindDescription_WithFieldInfo : TestsFor<AssemblyExplorer>
  {
    private Description description;

    public override void BeforeEachTest()
    {
      description = Target.FindDescription(typeof(ExampleC.InExampleC_1));
    }

    [Test]
    public void ShouldReturnADescription()
    {
      description.ShouldNotBeNull();
    }

    [Test]
    public void ShouldReturnCorrectName()
    {
      description.Name.ShouldEqual("InExampleC 1");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindDescription_WithClass : TestsFor<AssemblyExplorer>
  {
    private Description description;

    public override void BeforeEachTest()
    {
      FieldInfo fieldInfo = typeof(ExampleC.InExampleC_1).GetField("is_spec_1",
        BindingFlags.Instance | BindingFlags.NonPublic);
      description = Target.FindDescription(fieldInfo);
    }

    [Test]
    public void ShouldReturnADescription()
    {
      description.ShouldNotBeNull();
    }

    [Test]
    public void ShouldReturnOneSpecification()
    {
      description.Specifications.Count().ShouldEqual(1);
    }
    
    [Test]
    public void ShouldReturnOneSpecificationNamedCorrectly()
    {
      description.Specifications.First().Name.ShouldEqual("is spec 1");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindDescription_WithFirstDescriptionOfExampleAssemblyTests : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Description> descriptions;
    private Description description;

    public override void  BeforeEachTest()
    {
      descriptions = Target.FindDescriptionsIn(typeof(Account).Assembly);
      description =
        descriptions.Where(x => x.Name == "Transferring between accounts").FirstOrDefault();
      description.ShouldNotBeNull();
    }

    [Test]
    public void ShouldHaveThreeSpecifications()
    {
      description.Specifications.Count().ShouldEqual(3);
    }

    [Test]
    public void ShouldHaveSpecificationsWithCorrectItClauses()
    {
      var names = description.Specifications.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "should debit the from account by the amount transferred",
        "should credit the to account by the amount transferred",
        "should throw a System Exception");
    }

    [Test]
    public void ShouldHaveSpecificationsWithCorrectWhenClauses()
    {
      description.Specifications.Select(x => x.WhenClause).Distinct().ToList().ShouldContainOnly(
        "the transfer is made", "a transfer is made that is too large");
    }
  }
}
