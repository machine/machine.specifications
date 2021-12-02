using System;
using System.Globalization;
using System.Threading;

namespace Machine.Specifications.Should.Specs.Utility.Internal
{
    class when_comparing_two_long_unequal_strings_with_difference_in_the_middle
    {
        const string Expected =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

        const string Actual =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aaliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

        static readonly string ExpectedMessage =
            "  Expected string length 591 but was 592. Strings differ at index 123." + Environment.NewLine +
            "  Expected: \"...t ut labore et dolore magna aliquyam erat, sed diam volupt...\"" + Environment.NewLine +
            "  But was:  \"...t ut labore et dolore magna aaliquyam erat, sed diam volup...\"" + Environment.NewLine +
            "  -------------------------------------------^";

        static Exception exception;

        Because of = () => 
            exception = Catch.Exception(() => Actual.ShouldEqual(Expected));

        It should_report_a_styled_exception_message = () =>
            exception.Message.ShouldEqual(ExpectedMessage);

        It should_be_a_specification_exception = () =>
            exception.ShouldBeOfExactType<SpecificationException>();
    }

    class when_comparing_two_strings_with_different_length
    {
        const string Expected = "Hello world!";

        const string Actual = "Hello";

        static string ActualMessage;

        static readonly string ExpectedMessage =
          "  Expected string length 12 but was 5. Strings differ at index 5." + Environment.NewLine +
          "  Expected: \"Hello world!\"" + Environment.NewLine +
          "  But was:  \"Hello\"" + Environment.NewLine +
          "  ----------------^";

        Because of = () =>
            ActualMessage = Catch.Exception(() => Actual.ShouldEqual(Expected)).Message;

        It should_report_the_expected_message = () =>
            ActualMessage.ShouldEqual(ExpectedMessage);
    }

    class when_comparing_two_uneqal_strings_with_same_length
    {
        const string Actual = "Hello world!";

        const string Expected = "Hello World!";

        static string ActualMessage;

        static readonly string ExpectedMessage =
          "  String lengths are both 12. Strings differ at index 6." + Environment.NewLine +
          "  Expected: \"Hello World!\"" + Environment.NewLine +
          "  But was:  \"Hello world!\"" + Environment.NewLine +
          "  -----------------^";

        Because of = () =>
            ActualMessage = Catch.Exception(() => Actual.ShouldEqual(Expected)).Message;

        It should_report_the_expected_message = () =>
            ActualMessage.ShouldEqual(ExpectedMessage);
    }

    class when_comparing_two_uneqal_strings_with_difference_on_start
    {
        const string Actual = "Bello world!";

        const string Expected = "Hello World!";

        static string ActualMessage;

        static readonly string ExpectedMessage =
          "  String lengths are both 12. Strings differ at index 0." + Environment.NewLine +
          "  Expected: \"Hello World!\"" + Environment.NewLine +
          "  But was:  \"Bello world!\"" + Environment.NewLine +
          "  -----------^";

        Because of = () =>
        ActualMessage = Catch.Exception(() => Actual.ShouldEqual(Expected)).Message;

        It should_report_the_expected_message = () =>
            ActualMessage.ShouldEqual(ExpectedMessage);
    }

    class when_comparing_a_string_and_null
    {
        const string Actual = "Hello world!";

        const string Expected = null;

        static string ActualMessage;

        Because of = () =>
            ActualMessage = Catch.Exception(() => Actual.ShouldEqual(Expected)).Message;

        It should_report_a_formatted_message = () =>
            ActualMessage.ShouldEqual("  Expected: [null]" + Environment.NewLine + "  But was:  \"Hello world!\"");
    }

    class when_comparing_two_unequal_numbers
    {
        const double Actual = 4.5568;

        const double Expected = 4.5578;

        static string ActualMessage;

        static CultureInfo Culture;

        Establish context = () =>
        {
#if NETCOREAPP
            Culture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
#else
            Culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
#endif
        };

        Because of = () =>
        ActualMessage = Catch.Exception(() => Actual.ShouldEqual(Expected)).Message;

        It should_report_a_formatted_message = () =>
            ActualMessage.ShouldEqual("  Expected: [4.5578]" + Environment.NewLine + "  But was:  [4.5568]");

        Cleanup after = () => {
#if NETCOREAPP
            CultureInfo.CurrentCulture = Culture;
#else
            Thread.CurrentThread.CurrentCulture = Culture;
#endif
        };
    }

    class when_comparing_two_unequal_strings_with_difference_in_the_end
    {
        const string Actual =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt";

        const string Expected =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor iinvidunt";

        static readonly string ExpectedMessage =
            "  Expected string length 96 but was 95. Strings differ at index 88." + Environment.NewLine +
            "  Expected: \"...tur sadipscing elitr, sed diam nonumy eirmod tempor iinvidunt\"" + Environment.NewLine +
            "  But was:  \"...tur sadipscing elitr, sed diam nonumy eirmod tempor invidunt\"" + Environment.NewLine +
            "  -------------------------------------------------------------------^";

        static Exception Exception;

        Because of = () =>
            Exception = Catch.Exception(() => Actual.ShouldEqual(Expected));

        It should_report_a_styled_exception_message = () =>
            Exception.Message.ShouldEqual(ExpectedMessage);
    }

    class when_comparing_two_unequal_strings_with_difference_at_the_start
    {
        const string Actual =
            "LLorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt";

        const string Expected =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt";

        static readonly string ExpectedMessage =
            "  Expected string length 95 but was 96. Strings differ at index 1." + Environment.NewLine +
            "  Expected: \"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed ...\"" + Environment.NewLine +
            "  But was:  \"LLorem ipsum dolor sit amet, consetetur sadipscing elitr, sed...\"" + Environment.NewLine +
            "  ------------^";

        static Exception Exception;

        Because of = () =>
            Exception = Catch.Exception(() => Actual.ShouldEqual(Expected));

        It should_report_a_styled_exception_message = () =>
            Exception.Message.ShouldEqual(ExpectedMessage);
    }

    public class when_comparing_two_strings_with_different_lengths
    {
        static readonly string ExpectedMessage =
          "  Expected string length 3 but was 4. Strings differ at index 3." + Environment.NewLine +
          "  Expected: \"1.0\"" + Environment.NewLine +
          "  But was:  \"1.00\"" + Environment.NewLine +
          "  --------------^";

        static Exception Exception;

        Because of = () =>
            Exception = Catch.Exception(() => "1.00".ShouldEqual("1.0"));

        It should_report_a_styled_exception_message = () =>
            Exception.Message.ShouldEqual(ExpectedMessage);
    }
}
