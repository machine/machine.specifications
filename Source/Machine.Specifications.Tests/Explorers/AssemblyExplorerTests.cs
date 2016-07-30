using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Example;

using FluentAssertions;

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
      specifications = Target.FindContextsIn(typeof(Account).GetTypeInfo().Assembly);
    }

    [Test]
    public void ShouldReturnThreeContexts()
    {
      specifications.Count().Should().Be(3);
    }

    [Test]
    public void ShouldReturnThreeContextsNamedCorrectly()
    {
      var names = specifications.Select(x => x.Name).OrderBy(x => x).ToList();
      names.Should().BeEquivalentTo(
        new[]
    {
      "when a customer first views the account summary page",
      "when transferring between two accounts",
      "when transferring an amount larger than the balance of the from account"
    }.OrderBy(x => x).ToList()
        );
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindContexts_WithOneOfManyNamespaces : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Model.Context> descriptions;

    public override void BeforeEachTest()
    {
      descriptions = Target.FindContextsIn(typeof(ExampleA.InExampleA_1).GetTypeInfo().Assembly, "Machine.Specifications.ExampleA");
    }

    [Test]
    public void ShouldReturnTwoContexts()
    {
      descriptions.Count().Should().Be(2);
    }

    [Test]
    public void ShouldReturnTwoContextsNamedCorrectly()
    {
      var names = descriptions.Select(x => x.Name).ToList();
      names.Should().BeEquivalentTo(
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
      context.Should().NotBeNull();
    }

    [Test]
    public void ShouldReturnCorrectName()
    {
      context.Name.Should().Be("InExampleC 1");
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
      context.Should().NotBeNull();
    }

    [Test]
    public void ShouldReturnOneSpecification()
    {
      context.Specifications.Count().Should().Be(1);
    }

    [Test]
    public void ShouldReturnOneSpecificationNamedCorrectly()
    {
      context.Specifications.First().Name.Should().Be("is spec 1");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindContext_WithFirstContextOfExampleAssemblyTests : TestsFor<AssemblyExplorer>
  {
    private IEnumerable<Model.Context> descriptions;
    private Model.Context context;

    public override void BeforeEachTest()
    {
      descriptions = Target.FindContextsIn(typeof(Account).GetTypeInfo().Assembly);
      context =
        descriptions.Where(x => x.Name == "when transferring between two accounts").FirstOrDefault();
      context.Should().NotBeNull();
    }

    [Test]
    public void ShouldHaveTwoSpecifications()
    {
      context.Specifications.Count().Should().Be(2);
    }

    [Test]
    public void ShouldHaveSpecificationsWithCorrectItClauses()
    {
      var names = context.Specifications.Select(x => x.Name).ToList();
      names.Should().BeEquivalentTo(
        "should debit the from account by the amount transferred",
        "should credit the to account by the amount transferred");
    }
  }

  [TestFixture]
  public class AssemblyExplorer_FindAssemblyContextsIn_WithinAnAssembly : TestsFor<AssemblyExplorer>
  {
    private List<IAssemblyContext> assemblyContexts;

    public override void BeforeEachTest()
    {
      var assembly = typeof(AssemblyExplorer_FindAssemblyContextsIn_WithinAnAssembly).GetTypeInfo().Assembly;
      assemblyContexts = new List<IAssemblyContext>(Target.FindAssemblyContextsIn(assembly));
    }

    [Test]
    public void ShouldHaveOneAssemblyContext()
    {
      assemblyContexts.Count.Should().Be(1);
    }
  }

  public class TestAssemblyContext : IAssemblyContext
  {
    public void OnAssemblyStart()
    {
    }

    public void OnAssemblyComplete()
    {
    }
  }
}