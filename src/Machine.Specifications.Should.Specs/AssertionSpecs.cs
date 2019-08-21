using System;
using System.Collections.Generic;

namespace Machine.Specifications.Should.Specs
{
    [Subject(typeof(ShouldExtensionMethods))]
    public class when_checking_if_a_collection_contains_elements_that_match_a_func
    {
        static Exception Exception;
        static int[] Ints;

        Establish context = () => { Ints = new[] { 1, 2, 3 }; };

        Because of = () => { Exception = Catch.Exception(() => Ints.ShouldContain(x => x > 3)); };

        It should_print_the_func_description =
          () => Exception.Message.ShouldContain("Should contain elements conforming to: x => (x > 3)");
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_checking_if_a_collection_contains_elements_that_do_not_match_a_func
    {
        static Exception Exception;
        static int[] Ints;

        Establish context = () => { Ints = new[] { 1, 2, 3 }; };

        Because of = () => { Exception = Catch.Exception(() => Ints.ShouldNotContain(x => x < 3)); };

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

        Establish context = () => { Ints = new[] { 1, 2, 3 }; };

        Because of = () => { Exception = Catch.Exception(() => Ints.ShouldEachConformTo(x => x % 2 == 0)); };

        // Horribly inconsistent here. It seems like this spec might fail because linq expressions returns ((x % 2) = 0)
        // for the original ((x % 2) == 0). (fixed with .NET 4.0, therefore we use a regex match)

        It should_print_the_func_description =
          () => Exception.Message.ShouldMatch(@"Should contain only elements conforming to: x => \(\(x % 2\) (==|=) 0\)");

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
          () => SpecException.ShouldBeOfExactType<SpecificationException>();
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_a_null_string_is_asserted_on
    {
        static string AString;

        Establish context =
          () => AString = null;

        It should_fail_the__ShouldBeEmpty__assertion =
          () => Catch.Exception(() => AString.ShouldBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_fail_the__ShouldNotBeEmpty__assertion =
          () => Catch.Exception(() => AString.ShouldNotBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the__ShouldBeNull__assertion =
          () => Catch.Exception(() => AString.ShouldBeNull()).ShouldBeNull();

        It should_fail_the__ShouldNotBeNull__assertion =
          () => Catch.Exception(() => AString.ShouldNotBeNull()).ShouldBeOfExactType<SpecificationException>();
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_an_empty_string_is_asserted_on
    {
        static string AString;

        Establish context =
          () => AString = "";

        It should_pass_the__ShouldBeEmpty__assertion =
          () => Catch.Exception(() => AString.ShouldBeEmpty()).ShouldBeNull();

        It should_fail_the__ShouldNotBeEmpty__assertion =
          () => Catch.Exception(() => AString.ShouldNotBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_fail_the__ShouldBeNull__assertion =
          () => Catch.Exception(() => AString.ShouldBeNull()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the__ShouldNotBeNull__assertion =
          () => Catch.Exception(() => AString.ShouldNotBeNull()).ShouldBeNull();
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_an_non_empty_string_is_asserted_on
    {
        static string AString;

        Establish context =
          () => AString = "hey";

        It should_fail_the__ShouldBeEmpty__assertion =
          () => Catch.Exception(() => AString.ShouldBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the__ShouldNotBeEmpty__assertion =
          () => Catch.Exception(() => AString.ShouldNotBeEmpty()).ShouldBeNull();

        It should_fail_the__ShouldBeNull__assertion =
          () => Catch.Exception(() => AString.ShouldBeNull()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the__ShouldNotBeNull__assertion =
          () => Catch.Exception(() => AString.ShouldNotBeNull()).ShouldBeNull();
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_a_type_assertion_fails
    {
        static string AString;
        static SpecificationException Exception;

        Establish context = () => { AString = null; };

        Because of =
          () => Exception = (SpecificationException)Catch.Exception(() => AString.ShouldBeOfExactType<int>());

        It should_report_the_requested_type =
          () => Exception.Message.ShouldStartWith("Should be of type System.Int32");
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_checking_if_an_item_matches_a_func
    {
        static Exception Exception;
        static int Int;

        Establish context = () => { Int = 42; };

        Because of = () => { Exception = Catch.Exception(() => Int.ShouldMatch(x => x > 1)); };

        It should_succeed =
          () => Exception.ShouldBeNull();
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_checking_if_an_item_matches_a_func_and_the_check_fails
    {
        static Exception Exception;
        static int Int;

        Establish context = () => { Int = 42; };

        Because of = () => { Exception = Catch.Exception(() => Int.ShouldMatch(x => x > 50)); };

        It should_print_the_func_description =
          () => Exception.Message.ShouldContain("Should match expression [x => (x > 50)], but does not.");
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_comparing_different_classes_with_equals_for_equality
    {
        Because of = () => FiveFrancs = new Franc(5, "CHF");
        It should_equal_money_with_currency_set_to_francs = () => FiveFrancs.Equals(new Money(5, "CHF")).ShouldBeTrue();
        static Franc FiveFrancs;
    }

    [Subject(typeof(ShouldExtensionMethods))]
    public class when_comparing_different_classes_with_should_equal_for_equality
    {
        Because of = () => FiveFrancs = new Franc(5, "CHF");
        It should_be_ten_francs_when_multiplied_by_2 = () => FiveFrancs.Times(2).ShouldEqual(Money.Franc(10));
        It should_be_fifteen_francs_when_multiplied_by_3 = () => FiveFrancs.Times(3).ShouldEqual(Money.Franc(15));
        static Franc FiveFrancs;
    }

    public class Money : IEquatable<Money>
    {
        readonly int _amount;
        readonly string _currency;

        public Money(int amount, string currency)
        {
            _amount = amount;
            _currency = currency;
        }

        public Money Times(int multiplier)
        {
            return new Money(multiplier * _amount, _currency);
        }

        public static Money Franc(int amount)
        {
            return new Franc(amount, "CHF");
        }

        public bool Equals(Money other)
        {
            return _amount == other._amount && _currency == other._currency;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as Money);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_amount * 997) ^ _currency.GetHashCode();
            }
        }
    }

    public class Franc : Money
    {
        public Franc(int amount, string currency)
            : base(amount, currency)
        {
        }
    }
}