using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Example;
using Example.Random;
using FluentAssertions;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;

namespace Machine.Specifications.Specs.Explorers
{
    [Subject(typeof(AssemblyExplorer))]
    public class When_inspecting_internal_types_for_contexts
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> Contexts;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            Contexts = Explorer.FindContextsIn(typeof(tag).GetTypeInfo().Assembly, "Example.Random.Internal");

        It should_find_two_contexts = () =>
            Contexts.Count().Should().Be(2);
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindSpecifications_WithExampleAssemblyTests
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> contexts;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            contexts = Explorer.FindContextsIn(typeof(Account).GetTypeInfo().Assembly);

        It should_find_three_contexts = () =>
            contexts.Count().Should().Be(3);

        It should_return_three_contexts_named_correctly = () =>
            contexts
                .Select(x => x.Name)
                .OrderBy(x => x)
                .Should().BeEquivalentTo(
                    new[]
                    {
                        "when a customer first views the account summary page",
                        "when transferring between two accounts",
                        "when transferring an amount larger than the balance of the from account"
                    }.OrderBy(x => x)
                );
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContexts_WithOneOfManyNamespaces
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> contexts;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            contexts = Explorer.FindContextsIn(typeof(ExampleA.InExampleA_1).GetTypeInfo().Assembly, "Machine.Specifications.ExampleA");

        It should_find_two_contexts = () =>
            contexts.Count().Should().Be(2);

        It should_return_two_contexts_named_correctly = () =>
            contexts
                .Select(x => x.Name)
                .Should()
                .BeEquivalentTo(
                    "InExampleA 1",
                    "InExampleA 2");
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContext_WithFieldInfo
    {
        static AssemblyExplorer Explorer;
        static Context single_context;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            single_context = Explorer.FindContexts(typeof(ExampleC.InExampleC_1));

        It should_return_a_context = () =>
            single_context.Should().NotBeNull();

        It should_return_correct_name = () =>
            single_context.Name.Should().Be("InExampleC 1");
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContext_WithClass
    {
        static AssemblyExplorer Explorer;
        static Context single_context;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () => single_context =
            Explorer.FindContexts(typeof(ExampleC.InExampleC_1).GetField("is_spec_1", BindingFlags.Instance | BindingFlags.NonPublic));

        It should_return_a_context = () =>
            single_context.Should().NotBeNull();

        It should_return_one_specification = () =>
            single_context.Specifications.Count().Should().Be(1);

        It should_return_one_specification_named_correctly = () =>
            single_context.Specifications.First().Name.Should().Be("is spec 1");
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContext_WithFirstContextOfExampleAssemblyTests
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> contexts;
        static Context single_context;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
        {
            contexts = Explorer.FindContextsIn(typeof(Account).GetTypeInfo().Assembly);
            single_context = contexts.FirstOrDefault(x => x.Name == "when transferring between two accounts");
        };

        It should_return_a_context = () =>
            single_context.Should().NotBeNull();

        It should_return_two_specifications = () =>
            single_context.Specifications.Count().Should().Be(2);

        It should_have_specifications_with_correct_it_clauses = () =>
            single_context.Specifications
                .Select(x => x.Name)
                .Should()
                .BeEquivalentTo(
                    "should debit the from account by the amount transferred",
                    "should credit the to account by the amount transferred");
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindAssemblyContextsIn_WithinAnAssembly
    {
        static AssemblyExplorer Explorer;
        static List<IAssemblyContext> assemblyContexts;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            assemblyContexts = new List<IAssemblyContext>(
                Explorer.FindAssemblyContextsIn(typeof(AssemblyExplorer_FindAssemblyContextsIn_WithinAnAssembly).GetTypeInfo().Assembly));

        It should_have_one_assembly_context = () =>
            assemblyContexts.Count.Should().Be(1);
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
