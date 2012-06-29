﻿using System.Linq;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Specs.Utility
{
    [Subject(typeof(ObjectGraphHelper))]
    public class when_getting_an_object_graph
    {
        static object _obj;
        static ObjectGraphHelper.INode _result;

        public class with_property
        {
            Establish ctx = () => _obj = new {Property = "value"};

            Because of = () => _result = ObjectGraphHelper.GetGraph(_obj);

            It should_have_property = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                                .Where(m => m.Name == "Property").ShouldNotBeEmpty();

            It should_retrieve_value = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                                 .Where(m => m.Name == "Property").Single().ValueGetter().ShouldEqual("value");

            It should_return_a_key_value_node = () => _result.ShouldBeOfType<ObjectGraphHelper.KeyValueNode>();
        }

        public class with_field
        {
            Establish ctx = () =>
            {
                var obj = new ObjectWithField();
                obj.Field = "value";
                _obj = obj;
            };

            Because of = () => _result = ObjectGraphHelper.GetGraph(_obj);

            It should_have_field = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                             .Where(m => m.Name == "Field").ShouldNotBeEmpty();

            It should_retrieve_value = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                                 .Where(m => m.Name == "Field").Single().ValueGetter().ShouldEqual("value");

            It should_return_a_key_value_node = () => _result.ShouldBeOfType<ObjectGraphHelper.KeyValueNode>();
        }

        public class with_readonly_field
        {
            Establish ctx = () => _obj = new ObjectWithReadOnlyField("value");

            Because of = () => _result = ObjectGraphHelper.GetGraph(_obj);

            It should_have_field = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                             .Where(m => m.Name == "Field").ShouldNotBeEmpty();

            It should_retrieve_value = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                                 .Where(m => m.Name == "Field").Single().ValueGetter().ShouldEqual("value");

            It should_return_a_key_value_node = () => _result.ShouldBeOfType<ObjectGraphHelper.KeyValueNode>();
        }

        public class with_property_and_field
        {
            Establish ctx = () =>
            {
                var obj = new ObjectWithPropertyAndField();
                obj.Property = "value1";
                obj.Field = "value2";
                _obj = obj;
            };

            Because of = () => _result = ObjectGraphHelper.GetGraph(_obj);

            It should_have_field = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                             .Where(m => m.Name == "Field").ShouldNotBeEmpty();

            It should_have_property = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                                .Where(m => m.Name == "Property").ShouldNotBeEmpty();

            It should_retrieve_field_value = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                                       .Where(m => m.Name == "Field").Single().ValueGetter().ShouldEqual("value2");

            It should_retrieve_property_value = () => ((ObjectGraphHelper.KeyValueNode) _result).KeyValues
                                                          .Where(m => m.Name == "Property").Single().ValueGetter().ShouldEqual("value1");

            It should_return_a_key_value_node = () => _result.ShouldBeOfType<ObjectGraphHelper.KeyValueNode>();
        }
    }

    [Subject(typeof(ObjectGraphHelper))]
    public class when_getting_an_array_graph
    {
        static object _array;
        static ObjectGraphHelper.INode _result;

        Because of = () => _result = ObjectGraphHelper.GetGraph(_array);

        public class with_no_values
        {
            Establish ctx = () => _array = new string[] {};

            It should_return_an_array_node = () => _result.ShouldBeOfType<ObjectGraphHelper.ArrayNode>();
            It should_be_empty = () => ((ObjectGraphHelper.ArrayNode) _result).ValueGetters.ShouldBeEmpty();
        }
        
        public class with_values
        {
            Establish ctx = () => _array = new[] {"value1", "value2"};

            It should_return_an_array_node = () => _result.ShouldBeOfType<ObjectGraphHelper.ArrayNode>();
            It should_contain_value1 = () => ((ObjectGraphHelper.ArrayNode) _result).ValueGetters.ElementAt(0)().ShouldEqual("value1");
            It should_contain_value2 = () => ((ObjectGraphHelper.ArrayNode) _result).ValueGetters.ElementAt(1)().ShouldEqual("value2");
        }
    }

    [Subject(typeof(ObjectGraphHelper))]
    public class when_getting_a_literal_graph
    {
        static object _literal;
        static ObjectGraphHelper.INode _result;

        Because of = () => _result = ObjectGraphHelper.GetGraph(_literal);

        public class with_a_string
        {
            Establish ctx = () => _literal = "value";

            It should_return_a_literal_node = () => _result.ShouldBeOfType<ObjectGraphHelper.LiteralNode>();
            It should_have_value = () => ((ObjectGraphHelper.LiteralNode) _result).Value.ShouldEqual("value");
        }
        
        public class with_an_int
        {
            Establish ctx = () => _literal = 5;
            
            It should_return_a_literal_node = () => _result.ShouldBeOfType<ObjectGraphHelper.LiteralNode>();
            It should_have_value = () => ((ObjectGraphHelper.LiteralNode) _result).Value.ShouldEqual(5);
        }
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
}