using System;
using System.Collections.Generic;

namespace Machine.Specifications.Specs
{
  [Subject(typeof(ShouldExtensionMethods))]
  public class when_checking_if_a_collection_contains_elements_that_match_a_func
  {
    static Exception Exception;
    static int[] Ints;

    Establish context = () =>
    {
      Ints = new[] {1, 2, 3};
    };

    Because of = () =>
    {
      Exception = Catch.Exception(() => Ints.ShouldContain(x => x > 3));
    };

    It should_print_the_func_description =
      () => Exception.Message.ShouldContain("Should contain elements conforming to: x => (x > 3)");
  }
  
  [Subject(typeof(ShouldExtensionMethods))]
  public class when_checking_if_a_collection_contains_elements_that_do_not_match_a_func
  {
    static Exception Exception;
    static int[] Ints;

    Establish context = () =>
    {
      Ints = new[] {1, 2, 3};
    };

    Because of = () =>
    {
      Exception = Catch.Exception(() => Ints.ShouldNotContain(x => x < 3));
    };

    It should_print_the_func_description =
      () => Exception.Message.ShouldContain("No elements should conform to: x => (x < 3)");

    It should_print_the_elements_that_did_not_match =
      () => Exception.Message.ShouldMatch(@"does contain: {\s+\[1\],\s+\[2\]\s+}");
  }

  [Subject(typeof(ShouldExtensionMethods))]
  public class when_checking_if_a_collection_contains_only_elements_that_match_a_func
  {
    static Exception Exception;
    static int[] Ints;

    Establish context = () =>
    {
      Ints = new[] { 1, 2, 3 };
    };

    Because of = () =>
    {
      Exception = Catch.Exception(() => Ints.ShouldEachConformTo(x => x % 2 == 0));
    };

    It should_print_the_func_description =
      () => Exception.Message.ShouldContain("Should contain only elements conforming to: x => ((x % 2) = 0)");

    It should_print_the_elements_that_did_not_match =
      () => Exception.Message.ShouldMatch(@"the following items did not meet the condition: {\s+\[1\],\s+\[3\]\s+}");
  }

  [Subject(typeof(ShouldExtensionMethods))]
  public class when_a_list_contains_an_element_of_another_list
  {
    static Exception SpecException;
    static List<string> AList;
    static List<string> AnotherList;
    static string Element;

    Establish context = () =>
    {
      Element = "An Element";
      AList = new List<string> { Element };
      AnotherList = new List<string>(AList);
    };

    Because of =
      () => SpecException = Catch.Exception(() => AList.ShouldContain(AnotherList));

    It should_pass_the__ShouldContains__assertion =
      () => SpecException.ShouldBeNull();
  }

  [Subject(typeof(ShouldExtensionMethods))]
  public class when_a_list_not_contains_an_element_of_another_list
  {
    static Exception SpecException;
    static List<string> AList;
    static List<string> AnotherList;
    static string Element;
    static string AnotherElement;

    Establish context = () =>
    {
      Element = "An Element";
      AList = new List<string> { Element };

      AnotherElement = "Another Element";
      AnotherList = new List<string> { AnotherElement };
    };

    Because of =
      () => SpecException = Catch.Exception(() => AList.ShouldContain(AnotherList));

    It should_fail_the__ShouldContains__assertion =
      () => SpecException.ShouldBeOfType<SpecificationException>();
  }
}