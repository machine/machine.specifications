using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Specs.Utility
{
    [Subject(typeof(ObjectGraphHelper))]
    public class when_getting_an_object_graph
    {
        static object _obj;
        static IEnumerable<ObjectGraphHelper.Member> _result;

        public class with_property
        {
            Establish ctx = () => _obj = new {Property = "value"};

            Because of = () => _result = ObjectGraphHelper.GetGraph(_obj);

            It should_have_property = () => _result.Where(m => m.Name == "Property").ShouldNotBeEmpty();
            It should_retrieve_value = () => _result.Where(m => m.Name == "Property").Single().ValueGetter().ShouldEqual("value");
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

            It should_have_field = () => _result.Where(m => m.Name == "Field").ShouldNotBeEmpty();
            It should_retrieve_value = () => _result.Where(m => m.Name == "Field").Single().ValueGetter().ShouldEqual("value");
        }

        public class with_readonly_field
        {
            Establish ctx = () => _obj = new ObjectWithReadOnlyField("value");

            Because of = () => _result = ObjectGraphHelper.GetGraph(_obj);

            It should_have_field = () => _result.Where(m => m.Name == "Field").ShouldNotBeEmpty();
            It should_retrieve_value = () => _result.Where(m => m.Name == "Field").Single().ValueGetter().ShouldEqual("value");
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
            
            It should_have_property = () => _result.Where(m => m.Name == "Property").ShouldNotBeEmpty();
            It should_retrieve_property_value = () => _result.Where(m => m.Name == "Property").Single().ValueGetter().ShouldEqual("value1");
            It should_have_field = () => _result.Where(m => m.Name == "Field").ShouldNotBeEmpty();
            It should_retrieve_field_value = () => _result.Where(m => m.Name == "Field").Single().ValueGetter().ShouldEqual("value2");
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
        public string Property { get; set; }   
        public string Field;
    }
}