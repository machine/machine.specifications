using System;
using System.Collections.Generic;

namespace Machine.Specifications.Should.Specs
{
    [Subject(typeof(ShouldExtensionMethods))]
    class when_asserting_object_is_like_expected_key_values
    {
        static Exception exception;

        static Dummy obj;

        Establish context = () =>
            obj = new Dummy { Prop1 = "test", Prop2 = 2 };

        public class Dummy
        {
            public string Prop1 { get; set; }

            public int Prop2 { get; set; }
        }

        class with_correct_key_values
        {
            Because of = () =>
                exception = Catch.Exception(() => obj.ShouldBeLike(new { Prop1 = "test", Prop2 = 2 }));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class with_incorrect_key_value
        {
            Because of = () =>
                exception = Catch.Exception(() => obj.ShouldBeLike(new { Prop1 = "test2", Prop2 = 2 }));

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop1"":" + Environment.NewLine +
                    @"  Expected string length 5 but was 4. Strings differ at index 4." + Environment.NewLine +
                    @"  Expected: ""test2""" + Environment.NewLine +
                    @"  But was:  ""test""" + Environment.NewLine +
                    @"  ---------------^");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();
        }

        class with_missing_key
        {
            Because of = () =>
                exception = Catch.Exception(() => obj.ShouldBeLike(new { Prop1 = "test", Prop2 = 2, Prop3 = "other" }));

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop3"":" + Environment.NewLine +
                    @"  Expected: ""other""" + Environment.NewLine +
                    @"  But was:  Not Defined");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();
        }

        class with_multiple_errors
        {
            Because of = () =>
                exception = Catch.Exception(() => obj.ShouldBeLike(new { Prop1 = "test2", Prop2 = 3, Prop3 = "other" }));

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop1"":" + Environment.NewLine +
                    @"  Expected string length 5 but was 4. Strings differ at index 4." + Environment.NewLine +
                    @"  Expected: ""test2""" + Environment.NewLine +
                    @"  But was:  ""test""" + Environment.NewLine +
                    @"  ---------------^" + Environment.NewLine +
                    Environment.NewLine +
                    @"""Prop2"":" + Environment.NewLine +
                    @"  Expected: [3]" + Environment.NewLine +
                    @"  But was:  [2]" + Environment.NewLine +
                    Environment.NewLine +
                    @"""Prop3"":" + Environment.NewLine +
                    @"  Expected: ""other""" + Environment.NewLine +
                    @"  But was:  Not Defined");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();
        }

        class with_using_object_multiple_times_in_expected_object_graph
        {
            // Regression test for issue 17: ShouldBeLikeInternal() must mark <actual,expected> as visisted instead of simply marking <expected>.
            static Dummy a;

            static Dummy b;

            Establish ctx = () =>
            {
                a = new Dummy { Prop = "a" };
                b = new Dummy { Prop = "b" };
            };

            Because of = () =>
                exception = Catch.Exception(() => new { A = a, B = b  }.ShouldBeLike(new { A = a, B = a  }));

            It should_contain_message = () => exception.Message.ShouldEqual(
                @"""B.Prop"":"  + Environment.NewLine +
                @"  String lengths are both 1. Strings differ at index 0." + Environment.NewLine +
                @"  Expected: ""a""" + Environment.NewLine +
                @"  But was:  ""b""" + Environment.NewLine +
                @"  -----------^");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            public class Dummy
            {
                public string Prop { get; set; }
            }
        }

        class with_different_types
        {
            class and_object_should_equal_integer
            {
                Because of = () =>
                    exception = Catch.Exception(() => new object().ShouldEqual(5));

                It should_throw_specification_exception = () =>
                    exception.ShouldBeOfExactType<SpecificationException>();
            }

            class and_integer_should_equal_object
            {
                Because of = () =>
                    exception = Catch.Exception(() => 5.ShouldEqual(new object()));

                It should_throw_specification_exception = () =>
                    exception.ShouldBeOfExactType<SpecificationException>();
            }
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    class when_asserting_object_is_like_expected_array
    {
        static Exception exception;

        static object array;

        class with_correct_values
        {
            Establish ctx = () =>
                array = new[] { "value1", "value2" };

            Because of = () =>
                exception = Catch.Exception(() => array.ShouldBeLike(new[] { "value1", "value2" }));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class with_no_values
        {
            Establish ctx = () =>
                array = new string[] { };

            Because of = () =>
                exception = Catch.Exception(() => array.ShouldBeLike(new string[] { }));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class with_incorrect_values
        {
            Establish ctx = () =>
                array = new[] { "value1", "value2" };

            Because of = () =>
                exception = Catch.Exception(() => array.ShouldBeLike(new[] { "value1", "value3" }));

            It should_contain_message = () => exception.Message.ShouldEqual(
                @"""[1]"":" + Environment.NewLine +
                @"  String lengths are both 6. Strings differ at index 5." + Environment.NewLine +
                @"  Expected: ""value3""" + Environment.NewLine +
                @"  But was:  ""value2""" + Environment.NewLine +
                @"  ----------------^");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();
        }

        class with_incorrect_length
        {
            Establish ctx = () =>
                array = new[] { "value1", "value2" };

            Because of = () =>
                exception = Catch.Exception(() => array.ShouldBeLike(new[] { "value1" }));

            It should_contain_message = () => exception.Message.ShouldEqual(
                @""""":" + Environment.NewLine +
                @"  Expected: Sequence length of 1" + Environment.NewLine +
                @"  But was:  2");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();
        }

        class with_using_object_multiple_times_in_expected_array
        {
            // Regression test for issue 17: ShouldBeLikeInternal() must mark <actual,expected> as visisted instead of simply marking <expected>.
            static Dummy a;

            static Dummy b;

            Establish ctx = () =>
            {
                a = new Dummy { Prop = "a" };
                b = new Dummy { Prop = "b" };
                array = new[] { a, b };
            };

            Because of = () =>
                exception = Catch.Exception(() => array.ShouldBeLike(new[] { a, a }));

            It should_contain_message = () => exception.Message.ShouldEqual
               (@"""[1].Prop"":" + Environment.NewLine +
                @"  String lengths are both 1. Strings differ at index 0." + Environment.NewLine +
                @"  Expected: ""a""" + Environment.NewLine +
                @"  But was:  ""b""" + Environment.NewLine +
                @"  -----------^");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            public class Dummy
            {
                public string Prop { get; set; }
            }
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    class when_asserting_object_is_like_expected_nested_object
    {
        static Exception exception;

        static Dummy obj = new Dummy
        {
            Prop1 = "value1",
            Prop2 = 10,
            NestedProp = new NestedDummy { NestedProp1 = 5, NestedProp2 = "value2" }
        };

        public class Dummy
        {
            public string Prop1 { get; set; }

            public int Prop2 { get; set; }

            public NestedDummy NestedProp { get; set; }
        }

        public class NestedDummy
        {
            public int NestedProp1 { get; set; }

            public string NestedProp2 { get; set; }
        }

        class with_correct_values
        {
            Because of = () =>
                exception = Catch.Exception(() => obj.ShouldBeLike(
                    new
                    {
                        Prop1 = "value1",
                        Prop2 = 10,
                        NestedProp = new {NestedProp1 = 5, NestedProp2 = "value2"}
                    }));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class with_incorrect_values
        {
            Because of = () =>
                exception = Catch.Exception(() => obj.ShouldBeLike(
                    new
                    {
                        Prop1 = "value1",
                        Prop2 = 10,
                        NestedProp = new {NestedProp1 = 7, NestedProp2 = "value2"}
                    }));

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""NestedProp.NestedProp1"":" + Environment.NewLine +
                    @"  Expected: [7]" + Environment.NewLine +
                    @"  But was:  [5]");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();
        }

        class with_missing_value
        {
            Because of = () =>
                exception = Catch.Exception(() => obj.ShouldBeLike(
                    new
                    {
                        Prop1 = "value1",
                        Prop2 = 10,
                        NestedProp = new
                        {
                            NestedProp1 = 5,
                            NestedProp2 = "value2",
                            NestedProp3 = "value3"
                        }
                    }));

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""NestedProp.NestedProp3"":" + Environment.NewLine +
                    @"  Expected: ""value3""" + Environment.NewLine +
                    @"  But was:  Not Defined");

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    class when_asserting_that_two_objects_of_the_same_concrete_type_are_like_each_other
    {
        static Exception exception;

        static Dummy obj1;

        static Dummy obj2;

        Establish context = () =>
            obj1 = new Dummy { Prop1 = "test" };

        public class Dummy
        {
            public string Prop1 { get; set; }
        }

        class and_the_objects_are_similar
        {
            Establish context = () =>
                obj2 = new Dummy { Prop1 = "test" };

            Because of = () =>
                exception = Catch.Exception(() => obj1.ShouldBeLike(obj2));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class and_the_objects_are_different
        {
            Establish context = () =>
                obj2 = new Dummy { Prop1 = "different" };

            Because of = () =>
                exception = Catch.Exception(() => obj1.ShouldBeLike(obj2));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop1"":" + Environment.NewLine +
                    @"  Expected string length 9 but was 4. Strings differ at index 0." + Environment.NewLine +
                    @"  Expected: ""different""" + Environment.NewLine +
                    @"  But was:  ""test""" + Environment.NewLine +
                    @"  -----------^");
        }

        class and_the_objects_are_different_and_have_null_values
        {
            Establish context = () =>
                obj2 = new Dummy { Prop1 = null };

            Because of = () =>
                exception = Catch.Exception(() => obj1.ShouldBeLike(obj2));

            It should_throw_a_specification_exception = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop1"":" + Environment.NewLine +
                    @"  Expected: [null]" + Environment.NewLine +
                    @"  But was:  ""test""");
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    class when_asserting_that_two_objects_containing_arrays_as_properties_are_like_each_other
    {
        static Exception exception;

        static Dummy obj1;

        static Dummy obj2;

        public class Dummy
        {
            public int[] Prop1 { get; set; }
        }

        Establish context = () => obj1 = new Dummy { Prop1 = new[] { 1, 1, 1 } };

        class and_the_objects_are_similar
        {
            Establish context = () =>
                obj2 = new Dummy { Prop1 = new[] { 1, 1, 1 } };

            Because of = () =>
                exception = Catch.Exception(() => obj1.ShouldBeLike(obj2));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class and_the_objects_are_different
        {
            Establish context = () =>
                obj2 = new Dummy { Prop1 = new[] { 2, 2, 2 } };

            Because of = () =>
                exception = Catch.Exception(() => obj1.ShouldBeLike(obj2));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop1[0]"":" + Environment.NewLine +
                    @"  Expected: [2]" + Environment.NewLine +
                    @"  But was:  [1]" + Environment.NewLine +
                    Environment.NewLine +
                    @"""Prop1[1]"":" + Environment.NewLine +
                    @"  Expected: [2]" + Environment.NewLine +
                    @"  But was:  [1]" + Environment.NewLine +
                    Environment.NewLine +
                    @"""Prop1[2]"":" + Environment.NewLine +
                    @"  Expected: [2]" + Environment.NewLine +
                    @"  But was:  [1]");
        }

        class and_the_objects_are_different_and_have_null_values
        {
            Establish context = () =>
                obj2 = new Dummy { Prop1 = null };

            Because of = () =>
                exception = Catch.Exception(() => obj1.ShouldBeLike(obj2));

            It should_throw_a_specification_exception = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop1"":" + Environment.NewLine +
                    @"  Expected: [null]" + Environment.NewLine +
                    @"  But was:  System.Int32[]:" + Environment.NewLine +
                    @"{" + Environment.NewLine +
                    @"  [1]," + Environment.NewLine +
                    @"  [1]," + Environment.NewLine +
                    @"  [1]" + Environment.NewLine +
                    @"}");
        }

        class and_the_objects_are_different_and_the_actual_object_has_a_null_value
        {
            Establish context = () =>
                obj2 = new Dummy { Prop1 = null };

            Because of = () =>
                exception = Catch.Exception(() => obj2.ShouldBeLike(obj1));

            It should_throw_a_specification_exception = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(
                @"""Prop1"":" + Environment.NewLine +
                @"  Expected: System.Int32[]:" + Environment.NewLine +
                @"{" + Environment.NewLine +
                @"  [1]," + Environment.NewLine +
                @"  [1]," + Environment.NewLine +
                @"  [1]" + Environment.NewLine +
                @"}" + Environment.NewLine +
                Environment.NewLine +
                @"  But was:  [null]");
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    class when_asserting_that_two_objects_containing_collections_as_properties_are_like_each_other
    {
        static ListDummy first;

        static ListDummy second;

        static Exception exception;

        public class ListDummy
        {
            public List<string> Prop1 { get; set; }

            public HashSet<int> Prop2 { get; set; }

            public IEnumerable<char> Prop3 { get; set; }
        }

        Establish context = () =>
            first = new ListDummy
            {
                Prop1 = new List<string> {"hello", "world"},
                Prop2 = new HashSet<int> {1, 2, 3},
                Prop3 = new LinkedList<char>(new[] {'a', 'b', 'c'})
            };

        class and_the_objects_are_similar
        {
            Establish context = () =>
                second = new ListDummy
                {
                    Prop1 = new List<string> {"hello", "world"},
                    Prop2 = new HashSet<int> {1, 2, 3},
                    Prop3 = new LinkedList<char>(new[] {'a', 'b', 'c'})
                };

            Because of = () =>
                exception = Catch.Exception(() => first.ShouldBeLike(second));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class and_the_objects_differ
        {
            Establish context = () =>
                second = new ListDummy
                {
                    Prop1 = new List<string> {"hello", "world"},
                    Prop2 = new HashSet<int> {3, 2, 1},
                    Prop3 = new LinkedList<char>(new[] {'a', 'b', 'c'})
                };

            Because of = () =>
                exception = Catch.Exception(() => first.ShouldBeLike(second));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop2[0]"":" + Environment.NewLine +
                    @"  Expected: [3]" + Environment.NewLine +
                    @"  But was:  [1]" + Environment.NewLine +
                    Environment.NewLine +
                    @"""Prop2[2]"":" + Environment.NewLine +
                    @"  Expected: [1]" + Environment.NewLine +
                    @"  But was:  [3]");
        }

        class and_the_objects_differ_and_have_null_values
        {
            Establish context = () =>
            {
                first.Prop2 = null;

                second = new ListDummy
                {
                    Prop1 = new List<string> { "hello", "world" },
                    Prop2 = null,
                    Prop3 = null
                };
            };

            Because of = () =>
                exception = Catch.Exception(() => first.ShouldBeLike(second));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Prop3"":" + Environment.NewLine +
                    @"  Expected: [null]" + Environment.NewLine +
                    @"  But was:  System.Collections.Generic.LinkedList`1[System.Char]:" + Environment.NewLine +
                    @"{" + Environment.NewLine +
                    @"  [a]," + Environment.NewLine +
                    @"  [b]," + Environment.NewLine +
                    @"  [c]" + Environment.NewLine +
                    @"}");
        }
    }

    [Subject(typeof (ShouldExtensionMethods))]
    class when_node_with_circular_references
    {
        public class Node
        {
            public string Field;

            public Node Next;
        }

        static Exception exception;

        static Node actual_node;

        static Node expected_node;

        Establish ctx = () =>
        {
            actual_node = new Node { Field = "field1" };
            expected_node = new Node { Field = "field1" };
        };

        class and_the_objects_are_equal
        {
            Establish ctx = () =>
                actual_node.Next = actual_node;

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(actual_node));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class and_the_objects_are_similar
        {
            Establish ctx = () =>
            {
                actual_node.Next = actual_node;
                expected_node.Next = expected_node;
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(expected_node));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class and_the_objects_differ_by_string_field
        {
            Establish ctx = () =>
            {
                actual_node.Next = actual_node;
                expected_node.Next = expected_node;
                expected_node.Field = "field2";
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(expected_node));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Field"":" + Environment.NewLine +
                    @"  String lengths are both 6. Strings differ at index 5." + Environment.NewLine +
                    @"  Expected: ""field2""" + Environment.NewLine +
                    @"  But was:  ""field1""" + Environment.NewLine +
                    @"  ----------------^");
        }

        class and_expected_has_circular_reference
        {
            Establish ctx = () =>
            {
                actual_node.Next = null;
                expected_node.Next = expected_node;
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(expected_node));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Next"":" + Environment.NewLine +
                    @"  Expected: Machine.Specifications.Should.Specs.when_node_with_circular_references+Node" + Environment.NewLine +
                    @"  But was:  [null]");
        }

        class and_actual_has_circular_reference
        {
            Establish ctx = () =>
            {
                actual_node.Next = actual_node;
                expected_node.Next = null;
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(expected_node));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Next"":" + Environment.NewLine +
                    @"  Expected: [null]" + Environment.NewLine +
                    @"  But was:  Machine.Specifications.Should.Specs.when_node_with_circular_references+Node");
        }

        class and_the_object_graph_is_similar
        {
            Establish ctx = () =>
            {
                actual_node.Next = actual_node;
                var interNode = new Node { Field = "field1", Next = expected_node };
                expected_node.Next = interNode;
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(expected_node));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class and_the_objects_differ_by_referencing_node
        {
            Establish ctx = () =>
            {
                actual_node.Next = actual_node;
                var interNode = new Node {Field = "field3", Next = expected_node};
                expected_node.Next = interNode;
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(expected_node));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""Next.Field"":" + Environment.NewLine +
                    @"  String lengths are both 6. Strings differ at index 5." + Environment.NewLine +
                    @"  Expected: ""field3""" + Environment.NewLine +
                    @"  But was:  ""field1""" + Environment.NewLine +
                    @"  ----------------^");
        }

        class and_the_node_has_indirect_circular_reference
        {
            Establish ctx = () =>
            {
                var interNode = new Node {Field = "node3", Next = actual_node };
                actual_node.Next = expected_node;
                expected_node.Next = interNode;
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node.ShouldBeLike(actual_node));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }
    }

    [Subject(typeof (ShouldExtensionMethods))]
    class when_node_with_inner_static_node
    {
        public class Node
        {
            public string Field;

            public static readonly Node Inner = new Node();
        }

        static Exception exception;

        static Node actual_node;

        static Node expected_node;

        Establish ctx = () =>
        {
            actual_node = new Node { Field = "a" };
            expected_node = new Node { Field="a" };
        };

        Because of = () =>
            exception = Catch.Exception(() => actual_node.ShouldBeLike(expected_node));

        It should_not_throw = () =>
            exception.ShouldBeNull();
    }

    [Subject(typeof(ShouldExtensionMethods))]
    class when_complex_type_with_circular_references_are_in_collection
    {
        public class Node
        {
            public string Field;

            public Node Next;
        }

        static Exception exception;

        static Node node1;

        static Node node2;

        static List<Node> actual_node_list;

        static List<Node> expected_node_list;

        Establish ctx = () =>
        {
            node1 = new Node {Field = "node1Field"};
            node1.Next = node1;
            node2 = new Node {Field = "node2Field"};
            node2.Next = node2;
        };

        class and_the_elements_are_similar
        {
            Establish ctx = () =>
            {
                actual_node_list = new List<Node> { node1, node2 };
                expected_node_list = new List<Node> { node1, node2 };
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node_list.ShouldBeLike(expected_node_list));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }

        class and_the_elements_differ
        {
            Establish ctx = () =>
            {
                actual_node_list = new List<Node> { node1, node2 };
                var node3 = new Node {Field = "node3Field"};
                node3.Next = node3;
                expected_node_list = new List<Node> { node1, node3 };
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node_list.ShouldBeLike(expected_node_list));

            It should_throw = () =>
                exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual(
                    @"""[1].Field"":" + Environment.NewLine +
                    @"  String lengths are both 10. Strings differ at index 4." + Environment.NewLine +
                    @"  Expected: ""node3Field""" + Environment.NewLine +
                    @"  But was:  ""node2Field""" + Environment.NewLine +
                    @"  ---------------^");

        }

        class and_the_elements_reference_each_other
        {
            Establish ctx = () =>
            {
                node1.Next = node2;
                node2.Next = node1;
                actual_node_list = new List<Node> { node1, node2 };
                expected_node_list = new List<Node> { node1, node2 };
            };

            Because of = () =>
                exception = Catch.Exception(() => actual_node_list.ShouldBeLike(expected_node_list));

            It should_not_throw = () =>
                exception.ShouldBeNull();
        }
    }
}
