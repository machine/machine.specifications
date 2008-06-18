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
    private IEnumerable<Model.Context> specifications;

    public override void BeforeEachTest()
    {
      specifications = Target.FindContextsIn(typeof(Account).Assembly);
    }

    [Test]
    public void ShouldReturnOneContext()
    {
      specifications.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldReturnTwoContextsNamedCorrectly()
    {
      var names = specifications.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "When transferring between two accounts",
        "When transferring an amount larger than the balance of the from account"
        );
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindContexts_WithOneOfManyNamespaces : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Model.Context> descriptions;

    public override void BeforeEachTest()
    {
      descriptions = Target.FindContextsIn(typeof(ExampleA.InExampleA_1).Assembly, "Machine.Specifications.ExampleA");
    }

    [Test]
    public void ShouldReturnTwoContexts()
    {
      descriptions.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldReturnTwoContextsNamedCorrectly()
    {
      var names = descriptions.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "InExampleA 1",
        "InExampleA 2");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindContext_WithFieldInfo : TestsFor<AssemblyExplorer>
  {
    private Model.Context context;

    public override void BeforeEachTest()
    {
      context = Target.FindContexts(typeof(ExampleC.InExampleC_1));
    }

    [Test]
    public void ShouldReturnAContext()
    {
      context.ShouldNotBeNull();
    }

    [Test]
    public void ShouldReturnCorrectName()
    {
      context.Name.ShouldEqual("InExampleC 1");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindContext_WithClass : TestsFor<AssemblyExplorer>
  {
    private Model.Context context;

    public override void BeforeEachTest()
    {
      FieldInfo fieldInfo = typeof(ExampleC.InExampleC_1).GetField("is_spec_1",
        BindingFlags.Instance | BindingFlags.NonPublic);
      context = Target.FindContexts(fieldInfo);
    }

    [Test]
    public void ShouldReturnAContext()
    {
      context.ShouldNotBeNull();
    }

    [Test]
    public void ShouldReturnOneSpecification()
    {
      context.Specifications.Count().ShouldEqual(1);
    }
    
    [Test]
    public void ShouldReturnOneSpecificationNamedCorrectly()
    {
      context.Specifications.First().Name.ShouldEqual("is spec 1");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindContext_WithFirstContextOfExampleAssemblyTests : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Model.Context> descriptions;
    private Model.Context context;

    public override void  BeforeEachTest()
    {
      descriptions = Target.FindContextsIn(typeof(Account).Assembly);
      context =
        descriptions.Where(x => x.Name == "When transferring between two accounts").FirstOrDefault();
      context.ShouldNotBeNull();
    }

    [Test]
    public void ShouldHaveTwoSpecifications()
    {
      context.Specifications.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldHaveSpecificationsWithCorrectItClauses()
    {
      var names = context.Specifications.Select(x => x.Name).ToList();
      names.ShouldContainOnly(
        "should debit the from account by the amount transferred",
        "should credit the to account by the amount transferred");
    }
  }
}
