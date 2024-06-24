using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Reflection;

namespace Machine.Specifications.Should.Specs.Reflection
{
    [Subject(typeof(ObjectGraph))]
    class when_getting_an_object_graph
    {
        static object subject;

        static INode result;

        class with_property
        {
            Establish ctx = () =>
                subject = new { Property = "value" };

            Because of = () =>
                result = ObjectGraph.Get(subject);

            It should_have_property = () =>
                ((KeyValueNode) result).KeyValues.Where(m => m.Name == "Property").ShouldNotBeEmpty();

            It should_retrieve_value = () =>
                ((KeyValueNode) result).KeyValues.Single(m => m.Name == "Property").ValueGetter().ShouldEqual("value");

            It should_return_a_key_value_node = () =>
                result.ShouldBeOfExactType<KeyValueNode>();
        }

        class with_field
        {
            Establish ctx = () =>
            {
                var obj = new ObjectWithField();
                obj.Field = "value";
                subject = obj;
            };

            Because of = () =>
                result = ObjectGraph.Get(subject);

            It should_have_field = () =>
                ((KeyValueNode) result).KeyValues.Where(m => m.Name == "Field").ShouldNotBeEmpty();

            It should_retrieve_value = () =>
                ((KeyValueNode) result).KeyValues.Single(m => m.Name == "Field").ValueGetter().ShouldEqual("value");

            It should_return_a_key_value_node = () =>
                result.ShouldBeOfExactType<KeyValueNode>();
        }

        class with_readonly_field
        {
            Establish ctx = () =>
                subject = new ObjectWithReadOnlyField("value");

            Because of = () =>
                result = ObjectGraph.Get(subject);

            It should_have_field = () =>
                ((KeyValueNode) result).KeyValues.Where(m => m.Name == "Field").ShouldNotBeEmpty();

            It should_retrieve_value = () =>
                ((KeyValueNode) result).KeyValues.Single(m => m.Name == "Field").ValueGetter().ShouldEqual("value");

            It should_return_a_key_value_node = () =>
                result.ShouldBeOfExactType<KeyValueNode>();
        }

        class with_property_and_field
        {
            Establish ctx = () =>
            {
                var obj = new ObjectWithPropertyAndField();
                obj.Property = "value1";
                obj.Field = "value2";
                subject = obj;
            };

            Because of = () =>
                result = ObjectGraph.Get(subject);

            It should_have_field = () =>
                ((KeyValueNode) result).KeyValues.Where(m => m.Name == "Field").ShouldNotBeEmpty();

            It should_have_property = () =>
                ((KeyValueNode) result).KeyValues.Where(m => m.Name == "Property").ShouldNotBeEmpty();

            It should_retrieve_field_value = () =>
                ((KeyValueNode) result).KeyValues.Single(m => m.Name == "Field").ValueGetter().ShouldEqual("value2");

            It should_retrieve_property_value = () =>
                ((KeyValueNode) result).KeyValues.Single(m => m.Name == "Property").ValueGetter().ShouldEqual("value1");

            It should_return_a_key_value_node = () =>
                result.ShouldBeOfExactType<KeyValueNode>();
        }
    }

    [Subject(typeof(ObjectGraph))]
    class when_getting_a_sequence_graph_with_an_array
    {
        static object array;

        static INode result;

        Because of = () =>
            result = ObjectGraph.Get(array);

        public class with_no_values
        {
            Establish ctx = () =>
                array = new string[] { };

            It should_return_an_array_node = () =>
                result.ShouldBeOfExactType<SequenceNode>();

            It should_be_empty = () =>
                ((SequenceNode) result).ValueGetters.ShouldBeEmpty();
        }

        class with_values
        {
            Establish ctx = () =>
                array = new[] { "value1", "value2" };

            It should_return_an_array_node = () =>
                result.ShouldBeOfExactType<SequenceNode>();

            It should_contain_value1 = () =>
                ((SequenceNode) result).ValueGetters.ElementAt(0)().ShouldEqual("value1");

            It should_contain_value2 = () =>
                ((SequenceNode) result).ValueGetters.ElementAt(1)().ShouldEqual("value2");
        }
    }

    [Subject(typeof(ObjectGraph))]
    class when_getting_a_sequence_graph_with_an_enumerable
    {
        static IEnumerable<string> sequence;

        static INode result;

        Because of = () =>
            result = ObjectGraph.Get(sequence);

        class when_no_values
        {
            Establish ctx = () =>
                sequence = new List<string>();

            It should_return_an_array_node = () =>
                result.ShouldBeOfExactType<SequenceNode>();

            It should_be_empty = () =>
                ((SequenceNode) result).ValueGetters.ShouldBeEmpty();
        }

        class when_values
        {
            Establish ctx = () =>
                sequence = new List<string> { "value1", "value2" };

            It should_return_an_array_node = () =>
                result.ShouldBeOfExactType<SequenceNode>();

            It should_contain_value1 = () =>
                ((SequenceNode) result).ValueGetters.ElementAt(0)().ShouldEqual("value1");

            It should_contain_value2 = () =>
                ((SequenceNode) result).ValueGetters.ElementAt(1)().ShouldEqual("value2");
        }
    }

    [Subject(typeof(ObjectGraph))]
    class when_getting_a_literal_graph
    {
        static object literal;

        static INode result;

        Because of = () =>
            result = ObjectGraph.Get(literal);

        class with_a_string
        {
            Establish ctx = () =>
                literal = "value";

            It should_return_a_literal_node = () =>
                result.ShouldBeOfExactType<LiteralNode>();

            It should_have_value = () =>
                ((LiteralNode) result).Value.ShouldEqual("value");
        }

        class with_an_int
        {
            Establish ctx = () =>
                literal = 5;

            It should_return_a_literal_node = () =>
                result.ShouldBeOfExactType<LiteralNode>();

            It should_have_value = () =>
                ((LiteralNode) result).Value.ShouldEqual(5);
        }
    }

    [Subject(typeof(ObjectGraph))]
    class when_expected_inner_value_is_null
    {
        static Model actual_model;

        static Exception thrown_exception;

        Establish ctx = () =>
            actual_model = new Model { Inner = new InnerModel() };

        Because of = () =>
            thrown_exception = Catch.Exception(() => actual_model.ShouldBeLike(new Model() { Inner = null }));

        It should_throw_specification_exception = () =>
            thrown_exception.ShouldBeOfExactType<SpecificationException>();
    }

    [Subject(typeof(ObjectGraph))]
    class when_actual_inner_value_is_null
    {
        static Model actual_model;

        static Exception thrown_exception;

        Establish ctx = () =>
            actual_model = new Model { Inner = null };

        Because of = () =>
            thrown_exception = Catch.Exception(() => actual_model.ShouldBeLike(new Model() { Inner = new InnerModel() }));

        It should_throw_specification_exception = () =>
            thrown_exception.ShouldBeOfExactType<SpecificationException>();
    }

    [Subject(typeof(EquivalenceExtensions))]
    class when_actual_and_expected_value_are_null
    {
        static Model actual_model;

        static Exception thrown_exception;

        Establish ctx = () =>
            actual_model = new Model { Inner = null };

        Because of = () =>
            thrown_exception = Catch.Exception(() => actual_model.ShouldBeLike(new Model() { Inner = null }));

        It should_throw_specification_exception = () =>
            thrown_exception.ShouldBeNull();
    }

    class ObjectWithField
    {
        public string Field;
    }

    class ObjectWithReadOnlyField
    {
        public readonly string Field;

        public ObjectWithReadOnlyField(string field)
        {
            Field = field;
        }
    }

    class ObjectWithPropertyAndField
    {
        public string Field;

        public string Property { get; set; }
    }

    public class Model
    {
        public InnerModel Inner { get; set; }
    }

    public class InnerModel
    {
    }
}
