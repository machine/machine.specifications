using System;
using System.Linq;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Sdk;
using Machine.Specifications.Specs.Runner;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Specs.Factories
{
    [Subject(typeof(SpecificationFactory))]
    public class SpecificationFactory_CreateSpecification : RandomRunnerSpecs
    {
        static Type ContextWithSingleSpecification;
        static SpecificationFactory factory;
        static Specification specification;

        Establish context = () =>
        {
            ContextWithSingleSpecification = GetFramework("ContextWithSingleSpecification");

            factory = new SpecificationFactory();
        };

        Because of = () =>
        {
            var singleContext = new ContextFactory()
                .CreateContextFrom(Activator.CreateInstance(ContextWithSingleSpecification));

            var field = ContextWithSingleSpecification
                .GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName())
                .First();

            specification = factory.CreateSpecification(singleContext, field);
        };

        It should_have_correct_it_clause = () =>
            specification.Name.ShouldEqual("is a specification");

        It should_have_field_info = () =>
            specification.FieldInfo.Name.ShouldEqual("is_a_specification");
    }

    [Subject(typeof(SpecificationFactory))]
    public class SpecificationFactory_CreateThrowSpecification : RandomRunnerSpecs
    {
        static Type ContextWithThrowingSpecification;
        static SpecificationFactory factory;
        static Specification specification;

        Establish context = () =>
        {
            ContextWithThrowingSpecification = GetFramework("ContextWithThrowingSpecification");

            factory = new SpecificationFactory();
        };

        Because of = () =>
        {
            var field = ContextWithThrowingSpecification
                .GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName())
                .First();

            var context = new ContextFactory()
                .CreateContextFrom(Activator.CreateInstance(ContextWithThrowingSpecification));

            specification = factory.CreateSpecification(context, field);
        };

        It should_have_correct_it_clause = () =>
            specification.Name.ShouldEqual("should throw an exception");
    }

    [Subject(typeof(SpecificationFactory))]
    public class SpecificationFactory_CreateUndefinedSpecification : RandomRunnerSpecs
    {
        static Type ContextWithEmptySpecification;
        static SpecificationFactory factory;
        static Specification specification;

        Establish context = () =>
        {
            ContextWithEmptySpecification = GetFramework("ContextWithEmptySpecification");

            factory = new SpecificationFactory();
        };

        Because of = () =>
        {
            var field = ContextWithEmptySpecification
                .GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName())
                .First();

            var context = new ContextFactory()
                .CreateContextFrom(Activator.CreateInstance(ContextWithEmptySpecification));

            specification = factory.CreateSpecification(context, field);
        };

        It should_create_unknown_specification = () =>
            specification.IsDefined.ShouldBeFalse();
    }
}
