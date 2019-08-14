using System;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Specs.Factories
{
    //[Subject(typeof(SpecificationFactory))]
    //public class SpecificationFactory_CreateSpecification
    //{
    //    static SpecificationFactory factory;
    //    static Specification specification;

    //    Establish context = () =>
    //        factory = new SpecificationFactory();

    //    Because of = () =>
    //    {
    //        var singleContext = new ContextFactory()
    //            .CreateContextFrom(new ContextWithSingleSpecification());

    //        var field = typeof(ContextWithSingleSpecification)
    //            .GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName())
    //            .First();

    //        specification = factory.CreateSpecification(singleContext, field);
    //    };

    //    It should_have_correct_it_clause = () =>
    //        specification.Name.ShouldEqual("is a specification");

    //    It should_have_field_info = () =>
    //        specification.FieldInfo.Name.ShouldEqual("is_a_specification");
    //}

    //[Subject(typeof(SpecificationFactory))]
    //public class SpecificationFactory_CreateThrowSpecification
    //{
    //    static SpecificationFactory factory;
    //    static Specification specification;

    //    Establish context = () =>
    //        factory = new SpecificationFactory();

    //    Because of = () =>
    //    {
    //        var field = typeof(ContextWithThrowingSpecification)
    //            .GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName())
    //            .First();

    //        var context = new ContextFactory()
    //            .CreateContextFrom(new ContextWithThrowingSpecification());

    //        specification = factory.CreateSpecification(context, field);
    //    };

    //    It should_have_correct_it_clause = () =>
    //        specification.Name.ShouldEqual("should throw an exception");
    //}

    //[Subject(typeof(SpecificationFactory))]
    //public class SpecificationFactory_CreateUndefinedSpecification
    //{
    //    static SpecificationFactory factory;
    //    static Specification specification;

    //    Establish context = () =>
    //        factory = new SpecificationFactory();

    //    Because of = () =>
    //    {
    //        var field = typeof(ContextWithEmptySpecification)
    //            .GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName())
    //            .First();

    //        var context = new ContextFactory()
    //            .CreateContextFrom(new ContextWithEmptySpecification());

    //        specification = factory.CreateSpecification(context, field);
    //    };

    //    It should_create_unknown_specification = () =>
    //        specification.IsDefined.ShouldBeFalse();
    //}
}
