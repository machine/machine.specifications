using System;
using System.Collections.Generic;

namespace Machine.Specifications.Should.Specs
{
    [Subject(typeof(ShouldExtensionMethods))]
    public class when_asserting_object_is_like_expected_key_values
    {
        static Exception Exception;
        static Dummy Obj;

        Establish context = () => { Obj = new Dummy { Prop1 = "test", Prop2 = 2 }; };

        class Dummy
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
        }

        public class with_correct_key_values
        {
            Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new { Prop1 = "test", Prop2 = 2 })); };

            It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class with_incorrect_key_value
        {
            Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new { Prop1 = "test2", Prop2 = 2 })); };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1"":
  Expected string length 5 but was 4. Strings differ at index 4.
  Expected: ""test2""
  But was:  ""test""
  ---------------^");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }

        public class with_missing_key
        {
            Because of =
              () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new { Prop1 = "test", Prop2 = 2, Prop3 = "other" })); };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop3"":
  Expected: ""other""
  But was:  Not Defined");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }

        public class with_multiple_errors
        {
            Because of =
              () =>
              {
                  Exception = Catch.Exception(() => Obj.ShouldBeLike(new { Prop1 = "test2", Prop2 = 3, Prop3 = "other" }));
              };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1"":
  Expected string length 5 but was 4. Strings differ at index 4.
  Expected: ""test2""
  But was:  ""test""
  ---------------^

""Prop2"":
  Expected: [3]
  But was:  [2]

""Prop3"":
  Expected: ""other""
  But was:  Not Defined");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }

        public class with_using_object_multiple_times_in_expected_object_graph
        {
            // Regression test for issue 17: ShouldBeLikeInternal() must mark <actual,expected> as visisted instead of simply marking <expected>.

            Establish ctx = () =>
            {
                _a = new Dummy { Prop = "a" };
                _b = new Dummy { Prop = "b" };
            };

            Because of = () => { Exception = Catch.Exception(() => new { A = _a, B = _b  }.ShouldBeLike(new { A = _a, B = _a  })); };
      
            It should_contain_message = () => Exception.Message.ShouldEqual(@"""B.Prop"":
  String lengths are both 1. Strings differ at index 0.
  Expected: ""a""
  But was:  ""b""
  -----------^");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();

            static Dummy _a;
            static Dummy _b;

            class Dummy
            {
                public string Prop { get; set; }
            }
        }

        public class with_different_types
        {
            public class and_object_should_equal_integer
            {
                Because of = () => Exception = Catch.Exception(() => new object().ShouldEqual(5));
                It should_throw_specificationException = () => Exception.ShouldBeOfExactType<SpecificationException>();
            }

            public class and_integer_should_equal_object
            {
                Because of = () => Exception = Catch.Exception(() => 5.ShouldEqual(new object()));
                It should_throw_specificationException = () => Exception.ShouldBeOfExactType<SpecificationException>();
            }
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_asserting_object_is_like_expected_array
    {
        static Exception Exception;
        static object _array;

        public class with_correct_values
        {
            Establish ctx = () => _array = new[] { "value1", "value2" };
            Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] { "value1", "value2" })); };

            It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class with_no_values
        {
            Establish ctx = () => _array = new string[] { };
            Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new string[] { })); };

            It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class with_incorrect_values
        {
            Establish ctx = () => _array = new[] { "value1", "value2" };
            Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] { "value1", "value3" })); };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""[1]"":
  String lengths are both 6. Strings differ at index 5.
  Expected: ""value3""
  But was:  ""value2""
  ----------------^");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }

        public class with_incorrect_length
        {
            Establish ctx = () => _array = new[] { "value1", "value2" };
            Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] { "value1" })); };

            It should_contain_message = () => Exception.Message.ShouldEqual(@""""":
  Expected: Sequence length of 1
  But was:  2");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }

        public class with_using_object_multiple_times_in_expected_array
        {
            // Regression test for issue 17: ShouldBeLikeInternal() must mark <actual,expected> as visisted instead of simply marking <expected>.

            Establish ctx = () =>
            {
                _a = new Dummy { Prop = "a" };
                _b = new Dummy { Prop = "b" };
                _array = new[] { _a, _b };
            };

            Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] { _a, _a })); };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""[1].Prop"":
  String lengths are both 1. Strings differ at index 0.
  Expected: ""a""
  But was:  ""b""
  -----------^");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();

            static Dummy _a;
            static Dummy _b;

            class Dummy
            {
                public string Prop { get; set; }
            }
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_asserting_object_is_like_expected_nested_object
    {
        static Exception Exception;

        static Dummy _obj = new Dummy
        {
            Prop1 = "value1",
            Prop2 = 10,
            NestedProp = new NestedDummy { NestedProp1 = 5, NestedProp2 = "value2" }
        };

        class Dummy
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public NestedDummy NestedProp { get; set; }
        }

        class NestedDummy
        {
            public int NestedProp1 { get; set; }
            public string NestedProp2 { get; set; }
        }

        public class with_correct_values
        {
            Because of =
              () =>
              {
                  Exception =
                    Catch.Exception(
                                    () =>
                                      _obj.ShouldBeLike(
                                                        new
                                                        {
                                                            Prop1 = "value1",
                                                            Prop2 = 10,
                                                            NestedProp = new { NestedProp1 = 5, NestedProp2 = "value2" }
                                                        }));
              };

            It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class with_incorrect_values
        {
            Because of =
              () =>
              {
                  Exception =
                    Catch.Exception(
                                    () =>
                                      _obj.ShouldBeLike(
                                                        new
                                                        {
                                                            Prop1 = "value1",
                                                            Prop2 = 10,
                                                            NestedProp = new { NestedProp1 = 7, NestedProp2 = "value2" }
                                                        }));
              };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""NestedProp.NestedProp1"":
  Expected: [7]
  But was:  [5]");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }

        public class with_missing_value
        {
            Because of = () =>
            {
                Exception = Catch.Exception(() => _obj.ShouldBeLike(
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
            };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""NestedProp.NestedProp3"":
  Expected: ""value3""
  But was:  Not Defined");

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_asserting_that_two_objects_of_the_same_concrete_type_are_like_each_other
    {
        static Exception Exception;
        static Dummy Obj1;
        static Dummy Obj2;

        Establish context = () => { Obj1 = new Dummy { Prop1 = "test" }; };

        class Dummy
        {
            public string Prop1 { get; set; }
        }

        public class and_the_objects_are_similar
        {
            Establish context = () => { Obj2 = new Dummy { Prop1 = "test" }; };

            Because of = () => { Exception = Catch.Exception(() => Obj1.ShouldBeLike(Obj2)); };

            It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_the_objects_are_different
        {
            Establish context = () => { Obj2 = new Dummy { Prop1 = "different" }; };

            Because of = () => { Exception = Catch.Exception(() => Obj1.ShouldBeLike(Obj2)); };

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1"":
  Expected string length 9 but was 4. Strings differ at index 0.
  Expected: ""different""
  But was:  ""test""
  -----------^");
        }

        public class and_the_objects_are_different_and_have_null_values
        {
            Establish context = () => { Obj2 = new Dummy { Prop1 = null }; };

            Because of = () => { Exception = Catch.Exception(() => Obj1.ShouldBeLike(Obj2)); };

            It should_throw_a_specification_exception = () => Exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1"":
  Expected: [null]
  But was:  ""test""");
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_asserting_that_two_objects_containing_arrays_as_properties_are_like_each_other
    {
        static Exception Exception;
        static Dummy Obj1;
        static Dummy Obj2;

        class Dummy
        {
            public int[] Prop1 { get; set; }
        }

        Establish context = () => { Obj1 = new Dummy { Prop1 = new[] { 1, 1, 1 } }; };

        public class and_the_objects_are_similar
        {
            Establish context = () => { Obj2 = new Dummy { Prop1 = new[] { 1, 1, 1 } }; };

            Because of = () => { Exception = Catch.Exception(() => Obj1.ShouldBeLike(Obj2)); };

            It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_the_objects_are_different
        {
            Establish context = () => { Obj2 = new Dummy { Prop1 = new[] { 2, 2, 2 } }; };

            Because of = () => { Exception = Catch.Exception(() => Obj1.ShouldBeLike(Obj2)); };

            It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1[0]"":
  Expected: [2]
  But was:  [1]

""Prop1[1]"":
  Expected: [2]
  But was:  [1]

""Prop1[2]"":
  Expected: [2]
  But was:  [1]");
        }

        public class and_the_objects_are_different_and_have_null_values
        {
            Establish context = () => { Obj2 = new Dummy { Prop1 = null }; };

            Because of = () =>
            {
                Exception = Catch.Exception(() => Obj1.ShouldBeLike(Obj2));
            };

            It should_throw_a_specification_exception = () => Exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1"":
  Expected: [null]
  But was:  System.Int32[]:
{
  [1],
  [1],
  [1]
}");
        }

        public class and_the_objects_are_different_and_the_actual_object_has_a_null_value
        {
            Establish context = () => { Obj2 = new Dummy { Prop1 = null }; };

            Because of = () =>
            {
                Exception = Catch.Exception(() => Obj2.ShouldBeLike(Obj1));
            };

            It should_throw_a_specification_exception = () => Exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1"":
  Expected: System.Int32[]:
{
  [1],
  [1],
  [1]
}

  But was:  [null]");
        }
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_asserting_that_two_objects_containing_collections_as_properties_are_like_each_other
    {
        static ListDummy first;
        static ListDummy second;
        static Exception exception;

        class ListDummy
        {
            public List<string> Prop1 { get; set; }
            public HashSet<int> Prop2 { get; set; }
            public IEnumerable<char> Prop3 { get; set; }
        }

        Establish context = () =>
          first = new ListDummy
          {
              Prop1 = new List<string> { "hello", "world" },
              Prop2 = new HashSet<int> { 1, 2, 3 },
              Prop3 = new LinkedList<char>(new[] { 'a', 'b', 'c' })
          };

        public class and_the_objects_are_similar
        {
            Establish context = () =>
              second = new ListDummy
              {
                  Prop1 = new List<string> { "hello", "world" },
                  Prop2 = new HashSet<int> { 1, 2, 3 },
                  Prop3 = new LinkedList<char>(new[] { 'a', 'b', 'c' })
              };

            Because of = () => exception = Catch.Exception(() => first.ShouldBeLike(second));

            It should_not_throw = () => exception.ShouldBeNull();
        }

        public class and_the_objects_differ
        {
            Establish context = () =>
              second = new ListDummy
              {
                  Prop1 = new List<string> { "hello", "world" },
                  Prop2 = new HashSet<int> { 3, 2, 1 },
                  Prop3 = new LinkedList<char>(new[] { 'a', 'b', 'c' })
              };

            Because of = () => exception = Catch.Exception(() => first.ShouldBeLike(second));

            It should_throw = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(@"""Prop2[0]"":
  Expected: [3]
  But was:  [1]

""Prop2[2]"":
  Expected: [1]
  But was:  [3]");
        }

        public class and_the_objects_differ_and_have_null_values
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

            Because of = () => exception = Catch.Exception(() => first.ShouldBeLike(second));

            It should_throw = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(@"""Prop3"":
  Expected: [null]
  But was:  System.Collections.Generic.LinkedList`1[System.Char]:
{
  [a],
  [b],
  [c]
}");
        }
    }

    [Subject(typeof (ShouldExtensionMethods))]
    public class when_node_with_circular_references
    {
        class Node
        {
            public string Field;
            public Node Next;
        }

        static Exception exception;

        static Node actualNode;
        static Node expectedNode;

        Establish ctx = () =>
        {
            actualNode = new Node { Field = "field1" };
            expectedNode = new Node { Field = "field1" };
        };

        class and_the_objects_are_equal
        {
            Establish ctx = () => actualNode.Next = actualNode;

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(actualNode));

            It should_not_throw = () => exception.ShouldBeNull();
        }

        class and_the_objects_are_similar
        {
            Establish ctx = () =>
            {
                actualNode.Next = actualNode;
                expectedNode.Next = expectedNode;
            };

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(expectedNode));

            It should_not_throw = () => exception.ShouldBeNull();
        }

        class and_the_objects_differ_by_string_field
        {
            Establish ctx = () =>
            {
                actualNode.Next = actualNode;
                expectedNode.Next = expectedNode;
                expectedNode.Field = "field2";
            };

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(expectedNode));

            It should_throw = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(@"""Field"":
  String lengths are both 6. Strings differ at index 5.
  Expected: ""field2""
  But was:  ""field1""
  ----------------^");
        }

        class and_expected_has_circular_reference
        {
            Establish ctx = () =>
            {
                actualNode.Next = null;
                expectedNode.Next = expectedNode;
            };

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(expectedNode));

            It should_throw = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(@"""Next"":
  Expected: Machine.Specifications.Should.Specs.when_node_with_circular_references+Node
  But was:  [null]");
        }

        class and_actual_has_circular_reference
        {
            Establish ctx = () =>
            {
                actualNode.Next = actualNode;
                expectedNode.Next = null;
            };

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(expectedNode));

            It should_throw = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(@"""Next"":
  Expected: [null]
  But was:  Machine.Specifications.Should.Specs.when_node_with_circular_references+Node");
        }

        class and_the_object_graph_is_similar
        {
            Establish ctx = () =>
            {
                actualNode.Next = actualNode;
                var interNode = new Node { Field = "field1", Next = expectedNode };
                expectedNode.Next = interNode;
            };

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(expectedNode));

            It should_not_throw = () => exception.ShouldBeNull();
        }

        class and_the_objects_differ_by_referencing_node
        {
            Establish ctx = () =>
            {
                actualNode.Next = actualNode;
                var interNode = new Node {Field = "field3", Next = expectedNode};
                expectedNode.Next = interNode;
            };

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(expectedNode));

            It should_throw = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(@"""Next.Field"":
  String lengths are both 6. Strings differ at index 5.
  Expected: ""field3""
  But was:  ""field1""
  ----------------^");
        }

        class and_the_node_has_indirect_circular_reference
        {
            Establish ctx = () =>
            {
                var interNode = new Node {Field = "node3", Next = actualNode };
                actualNode.Next = expectedNode;
                expectedNode.Next = interNode;
            };

            Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(actualNode));

            It should_not_throw = () => exception.ShouldBeNull();
        }
    }

    [Subject(typeof (ShouldExtensionMethods))]
    public class when_node_with_inner_static_node
    {
        class Node
        {
            public string Field;
            public static readonly Node Inner = new Node();
        }

        static Exception exception;
        static Node actualNode;
        static Node expectedNode;

        Establish ctx = () =>
        {
            actualNode = new Node { Field = "a" };
            expectedNode = new Node { Field="a" };
        };

        Because of = () => exception = Catch.Exception(() => actualNode.ShouldBeLike(expectedNode));

        It should_not_throw = () => exception.ShouldBeNull();
    }

    [Subject(typeof(ShouldExtensionMethods))]
    class when_complex_type_with_circular_references_are_in_collection
    {
        class Node
        {
            public string Field;
            public Node Next;
        }

        static Exception exception;

        static Node node1;
        static Node node2;

        static List<Node> actualNodeList;
        static List<Node> expectedNodeList;

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
                actualNodeList = new List<Node> { node1, node2 };
                expectedNodeList = new List<Node> { node1, node2 };
            };

            Because of = () => exception = Catch.Exception(() => actualNodeList.ShouldBeLike(expectedNodeList));

            It should_not_throw = () => exception.ShouldBeNull();
        }

        class and_the_elements_differ
        {
            Establish ctx = () =>
            {
                actualNodeList = new List<Node> { node1, node2 };
                var node3 = new Node {Field = "node3Field"};
                node3.Next = node3;
                expectedNodeList = new List<Node> { node1, node3 };
            };

            Because of = () => exception = Catch.Exception(() => actualNodeList.ShouldBeLike(expectedNodeList));

            It should_throw = () => exception.ShouldBeOfExactType<SpecificationException>();

            It should_contain_message = () => exception.Message.ShouldEqual(@"""[1].Field"":
  String lengths are both 10. Strings differ at index 4.
  Expected: ""node3Field""
  But was:  ""node2Field""
  ---------------^");

        }

        class and_the_elements_reference_each_other
        {
            Establish ctx = () =>
            {
                node1.Next = node2;
                node2.Next = node1;
                actualNodeList = new List<Node> { node1, node2 };
                expectedNodeList = new List<Node> { node1, node2 };
            };

            Because of = () => exception = Catch.Exception(() => actualNodeList.ShouldBeLike(expectedNodeList));

            It should_not_throw = () => exception.ShouldBeNull();
        }
    }
}