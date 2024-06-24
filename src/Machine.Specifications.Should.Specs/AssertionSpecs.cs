using System;
using System.Collections.Generic;

namespace Machine.Specifications.Should.Specs
{
    [Subject(typeof(EnumerableExtensions))]
    class when_checking_if_a_collection_contains_elements_that_match_a_func
    {
        static Exception exception;

        static int[] ints;

        Establish context = () =>
            ints = new[] { 1, 2, 3 };

        Because of = () =>
            exception = Catch.Exception(() => ints.ShouldContain(x => x > 3));

        It should_print_the_func_description = () =>
            exception.Message.ShouldContain("Should contain elements conforming to: x => (x > 3)");
    }

    [Subject(typeof(EnumerableExtensions))]
    class when_checking_if_a_collection_contains_elements_that_do_not_match_a_func
    {
        static Exception exception;

        static int[] ints;

        Establish context = () =>
            ints = new[] { 1, 2, 3 };

        Because of = () =>
            exception = Catch.Exception(() => ints.ShouldNotContain(x => x < 3));

        It should_print_the_func_description = () =>
            exception.Message.ShouldContain("No elements should conform to: x => (x < 3)");

        It should_print_the_elements_that_did_not_match = () =>
            exception.Message.ShouldMatch(@"does contain: {\s+\[1\],\s+\[2\]\s+}");
    }

    [Subject(typeof(EnumerableExtensions))]
    class when_checking_if_a_collection_contains_only_elements_that_match_a_func
    {
        static Exception exception;

        static int[] ints;

        Establish context = () =>
            ints = new[] { 1, 2, 3 };

        Because of = () =>
            exception = Catch.Exception(() => ints.ShouldEachConformTo(x => x % 2 == 0));

        // Horribly inconsistent here. It seems like this spec might fail because linq expressions returns ((x % 2) = 0)
        // for the original ((x % 2) == 0). (fixed with .NET 4.0, therefore we use a regex match)

        It should_print_the_func_description = () =>
            exception.Message.ShouldMatch(@"Should contain only elements conforming to: x => \(\(x % 2\) (==|=) 0\)");

        It should_print_the_elements_that_did_not_match = () =>
            exception.Message.ShouldMatch(@"the following items did not meet the condition: {\s+\[1\],\s+\[3\]\s+}");
    }

    [Subject(typeof(EnumerableExtensions))]
    class when_a_list_contains_an_element_of_another_list
    {
        static Exception spec_exception;

        static List<string> a_list;

        static List<string> another_list;

        static string element;

        Establish context = () =>
        {
            element = "An Element";
            a_list = new List<string> { element };
            another_list = new List<string>(a_list);
        };

        Because of = () =>
            spec_exception = Catch.Exception(() => a_list.ShouldContain(another_list));

        It should_pass_the_should_contains_assertion = () =>
            spec_exception.ShouldBeNull();
    }

    [Subject(typeof(EnumerableExtensions))]
     class when_a_list_not_contains_an_element_of_another_list
    {
        static Exception spec_exception;

        static List<string> a_list;

        static List<string> another_list;

        static string element;

        static string another_element;

        Establish context = () =>
        {
            element = "An Element";
            a_list = new List<string> { element };

            another_element = "Another Element";
            another_list = new List<string> { another_element };
        };

        Because of = () =>
            spec_exception = Catch.Exception(() => a_list.ShouldContain(another_list));

        It should_fail_the_should_contains_assertion = () =>
            spec_exception.ShouldBeOfExactType<SpecificationException>();
    }

    [Subject(typeof(StringExtensions))]
    class when_a_null_string_is_asserted_on
    {
        static string a_string;

        Establish context = () =>
            a_string = null;

        It should_fail_the_should_be_empty_assertion = () =>
            Catch.Exception(() => a_string.ShouldBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_fail_the_should_not_be_empty_assertion = () =>
            Catch.Exception(() => a_string.ShouldNotBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the_should_be_null_assertion = () =>
            Catch.Exception(() => a_string.ShouldBeNull()).ShouldBeNull();

        It should_fail_the_should_not_be_null_assertion = () =>
            Catch.Exception(() => a_string.ShouldNotBeNull()).ShouldBeOfExactType<SpecificationException>();
    }

    [Subject(typeof(StringExtensions))]
    class when_an_empty_string_is_asserted_on
    {
        static string a_string;

        Establish context = () =>
            a_string = "";

        It should_pass_the_should_be_empty_assertion = () =>
            Catch.Exception(() => a_string.ShouldBeEmpty()).ShouldBeNull();

        It should_fail_the_should_not_be_empty_assertion = () =>
            Catch.Exception(() => a_string.ShouldNotBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_fail_the_should_be_null_assertion = () =>
            Catch.Exception(() => a_string.ShouldBeNull()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the_should_not_be_null_assertion = () =>
            Catch.Exception(() => a_string.ShouldNotBeNull()).ShouldBeNull();
    }

    [Subject(typeof(StringExtensions))]
    class when_an_non_empty_string_is_asserted_on
    {
        static string a_string;

        Establish context = () =>
            a_string = "hey";

        It should_fail_the_should_be_empty_assertion = () =>
            Catch.Exception(() => a_string.ShouldBeEmpty()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the_should_not_be_empty_assertion = () =>
            Catch.Exception(() => a_string.ShouldNotBeEmpty()).ShouldBeNull();

        It should_fail_the_should_be_null_assertion = () =>
            Catch.Exception(() => a_string.ShouldBeNull()).ShouldBeOfExactType<SpecificationException>();

        It should_pass_the_should_not_be_null_assertion = () =>
            Catch.Exception(() => a_string.ShouldNotBeNull()).ShouldBeNull();
    }

    [Subject(typeof(TypeExtensions))]
    class when_a_type_assertion_fails
    {
        static string a_string;

        static SpecificationException exception;

        Establish context = () =>
            a_string = null;

        Because of = () =>
            exception = (SpecificationException)Catch.Exception(() => a_string.ShouldBeOfExactType<int>());

        It should_report_the_requested_type = () =>
            exception.Message.ShouldStartWith("Should be of type System.Int32");
    }

    [Subject(typeof(EqualityExtensions))]
    class when_checking_if_an_item_matches_a_func
    {
        static Exception exception;

        static int value;

        Establish context = () =>
            value = 42;

        Because of = () =>
            exception = Catch.Exception(() => value.ShouldMatch(x => x > 1));

        It should_succeed = () =>
            exception.ShouldBeNull();
    }

    [Subject(typeof(EqualityExtensions))]
    class when_checking_if_an_item_matches_a_func_and_the_check_fails
    {
        static Exception exception;

        static int value;

        Establish context = () =>
            value = 42;

        Because of = () =>
            exception = Catch.Exception(() => value.ShouldMatch(x => x > 50));

        It should_print_the_func_description = () =>
            exception.Message.ShouldContain("Should match expression [x => (x > 50)], but does not.");
    }

    [Subject(typeof(EqualityExtensions))]
    class when_comparing_different_classes_with_equals_for_equality
    {
        static Franc five_francs;

        Because of = () =>
            five_francs = new Franc(5, "CHF");

        It should_equal_money_with_currency_set_to_francs = () =>
            five_francs.Equals(new Money(5, "CHF")).ShouldBeTrue();
    }

    [Subject(typeof(EqualityExtensions))]
    class when_comparing_different_classes_with_should_equal_for_equality
    {
        static Franc five_francs;

        Because of = () =>
            five_francs = new Franc(5, "CHF");

        It should_be_ten_francs_when_multiplied_by_2 = () =>
            five_francs.Times(2).ShouldEqual(Money.Franc(10));

        It should_be_fifteen_francs_when_multiplied_by_3 = () =>
            five_francs.Times(3).ShouldEqual(Money.Franc(15));
    }

    public class Money : IEquatable<Money>
    {
        public Money(int amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public int Amount { get; }

        public string Currency { get; }

        public Money Times(int multiplier)
        {
            return new Money(multiplier * Amount, Currency);
        }

        public static Money Franc(int amount)
        {
            return new Franc(amount, "CHF");
        }

        public bool Equals(Money other)
        {
            return other != null && Amount == other.Amount && Currency == other.Currency;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Money);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Amount * 997) ^ Currency.GetHashCode();
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
