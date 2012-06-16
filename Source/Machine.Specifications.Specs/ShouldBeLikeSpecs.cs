using System;

namespace Machine.Specifications.Specs
{
  [Subject(typeof(ShouldExtensionMethods))]
  public class when_asserting_object_is_like_expected_key_values
  {
    static Exception Exception;
    static Dummy Obj;

    Establish context = () => { Obj = new Dummy {Prop1 = "test", Prop2 = 2}; };

    class Dummy
    {
      public string Prop1 { get; set; }
      public int Prop2 { get; set; }
    }

    public class with_correct_key_values
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test", Prop2 = 2})); };

      It should_not_throw = () => Exception.ShouldBeNull();
    }

    public class with_incorrect_key_value
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test2", Prop2 = 2})); };

      It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop1"":
  Expected string length 5 but was 4. Strings differ at index 4.
  Expected: ""test2""
  But was:  ""test""
  ---------------^");

      It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
    }

    public class with_missing_key
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test", Prop2 = 2, Prop3 = "other"})); };

      It should_contain_message = () => Exception.Message.ShouldEqual(@"""Prop3"":
  Expected: ""other""
  But was:  Not Defined");

      It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
    }

    public class with_multiple_errors
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test2", Prop2 = 3, Prop3 = "other"})); };

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
      Establish ctx = () => _array = new[] {"value1", "value2"};
      Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] {"value1", "value2"})); };

      It should_not_throw = () => Exception.ShouldBeNull();
    }

    public class with_no_values
    {
      Establish ctx = () => _array = new string[] {};
      Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new string[] {})); };

      It should_not_throw = () => Exception.ShouldBeNull();
    }

    public class with_incorrect_values
    {
      Establish ctx = () => _array = new[] {"value1", "value2"};
      Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] {"value1", "value3"})); };

      It should_contain_message = () => Exception.Message.ShouldEqual(@"""[1]"":
  String lengths are both 6. Strings differ at index 5.
  Expected: ""value3""
  But was:  ""value2""
  ----------------^");

      It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
    }

    public class with_incorrect_length
    {
      Establish ctx = () => _array = new[] {"value1", "value2"};
      Because of = () => { Exception = Catch.Exception(() => _array.ShouldBeLike(new[] {"value1"})); };

      It should_contain_message = () => Exception.Message.ShouldEqual(@""""":
  Expected: Array length of 1
  But was:  2");

      It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
    }
  }

  [Subject(typeof(ShouldExtensionMethods))]
  public class when_asserting_object_is_like_expected_nested_object
  {
    static Exception Exception;
    static Dummy _obj = new Dummy {Prop1 = "value1", Prop2 = 10, NestedProp = new NestedDummy {NestedProp1 = 5, NestedProp2 = "value2"}};

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
      Because of = () => { Exception = Catch.Exception(() => _obj.ShouldBeLike(new {Prop1 = "value1", Prop2 = 10, NestedProp = new {NestedProp1 = 5, NestedProp2 = "value2"}})); };

      It should_not_throw = () => Exception.ShouldBeNull();
    }

    public class with_incorrect_values
    {
      Because of = () => { Exception = Catch.Exception(() => _obj.ShouldBeLike(new {Prop1 = "value1", Prop2 = 10, NestedProp = new {NestedProp1 = 7, NestedProp2 = "value2"}})); };

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
}