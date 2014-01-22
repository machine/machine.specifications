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

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
        }

        public class with_missing_key
        {
            Because of =
              () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new { Prop1 = "test", Prop2 = 2, Prop3 = "other" })); };

            It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop3"":
  Expected: ""other""
  But was:  Not Defined");

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
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

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
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

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
        }

        public class with_incorrect_length
        {
            Establish ctx = () => _array = new[] { "value1", "value2" };
            Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] { "value1" })); };

            It should_contain_message = () => Exception.Message.ShouldEqual(@""""":
  Expected: Sequence length of 1
  But was:  2");

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
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

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
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

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
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

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();

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

            It should_throw_a_specification_exception = () => Exception.ShouldBeOfType<SpecificationException>();

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

            It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();

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

            It should_throw_a_specification_exception = () => Exception.ShouldBeOfType<SpecificationException>();

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

            It should_throw_a_specification_exception = () => Exception.ShouldBeOfType<SpecificationException>();

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

            It should_throw = () => exception.ShouldBeOfType<SpecificationException>();

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

            It should_throw = () => exception.ShouldBeOfType<SpecificationException>();

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
}