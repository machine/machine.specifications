using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Specs.Runner;

namespace Machine.Specifications.Specs.Explorers
{
    [Subject(typeof(AssemblyExplorer))]
    public class When_inspecting_internal_types_for_contexts : RandomRunnerSpecs
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> Contexts;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            Contexts = Explorer.FindContextsIn(GetAssembly(), "Example.Random.Internal");

        It should_find_two_contexts = () =>
            Contexts.Count().ShouldEqual(2);
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindSpecifications_WithExampleAssemblyTests : ExampleRunnerSpecs
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> contexts;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            contexts = Explorer.FindContextsIn(Assembly.LoadFile(AssemblyPath));

        It should_find_three_contexts = () =>
            contexts.Count().ShouldEqual(3);

        It should_return_three_contexts_named_correctly = () =>
            contexts
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ShouldContainOnly(
                    new[]
                    {
                        "when a customer first views the account summary page",
                        "when transferring between two accounts",
                        "when transferring an amount larger than the balance of the from account"
                    }.OrderBy(x => x)
                );
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContexts_WithOneOfManyNamespaces : RandomRunnerSpecs
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> contexts;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
            contexts = Explorer.FindContextsIn(GetAssembly(), "Machine.Specifications.ExampleA");

        It should_find_two_contexts = () =>
            contexts.Count().ShouldEqual(2);

        It should_return_two_contexts_named_correctly = () =>
            contexts
                .Select(x => x.Name)
                .ShouldContainOnly(
                    "InExampleA 1",
                    "InExampleA 2");
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContext_WithFieldInfo : RandomRunnerSpecs
    {
        static AssemblyExplorer Explorer;
        static Context single_context;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
        {
            var type = GetFramework("ExampleC.InExampleC_1");

            single_context = Explorer.FindContexts(type);
        };

        It should_return_a_context = () =>
            single_context.ShouldNotBeNull();

        It should_return_correct_name = () =>
            single_context.Name.ShouldEqual("InExampleC 1");
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContext_WithClass : RandomRunnerSpecs
    {
        static AssemblyExplorer Explorer;
        static Context single_context;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
        {
            var type = GetFramework("ExampleC.InExampleC_1");

            single_context = Explorer.FindContexts(type.GetField("is_spec_1", BindingFlags.Instance | BindingFlags.NonPublic));
        };

        It should_return_a_context = () =>
            single_context.ShouldNotBeNull();

        It should_return_one_specification = () =>
            single_context.Specifications.Count().ShouldEqual(1);

        It should_return_one_specification_named_correctly = () =>
            single_context.Specifications.First().Name.ShouldEqual("is spec 1");
    }

    [Subject(typeof(AssemblyExplorer))]
    public class AssemblyExplorer_FindContext_WithFirstContextOfExampleAssemblyTests : ExampleRunnerSpecs
    {
        static AssemblyExplorer Explorer;
        static IEnumerable<Context> contexts;
        static Context single_context;

        Establish context = () =>
            Explorer = new AssemblyExplorer();

        Because of = () =>
        {
            contexts = Explorer.FindContextsIn(Assembly.LoadFile(AssemblyPath));
            single_context = contexts.FirstOrDefault(x => x.Name == "when transferring between two accounts");
        };

        It should_return_a_context = () =>
            single_context.ShouldNotBeNull();

        It should_return_two_specifications = () =>
            single_context.Specifications.Count().ShouldEqual(2);

        It should_have_specifications_with_correct_it_clauses = () =>
            single_context.Specifications
                .Select(x => x.Name)
                .ShouldContainOnly(
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
            assemblyContexts.Count.ShouldEqual(1);
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

    public class TestContext : ITestContext
    {
        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
        }

        public void OnFatalError(ExceptionResult exception)
        {
        }
    }
}
