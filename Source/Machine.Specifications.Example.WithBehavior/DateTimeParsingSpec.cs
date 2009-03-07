using System;

// This is a rather contrived example, but I hope it shows what you can do with it.
// I like the concept of MbUnit's RowTest and TypeFixture very much, especially when it comes to
// testing the same logic with different input patterns/files, etc.

namespace Machine.Specifications.Example.WithBehavior
{
  [Subject("Date time parsing")]
  public class when_a_date_is_parsed_with_the_regular_expression_parser : with_string_parser
  {
    Establish context = () => { Parser = new RegexParser(); };

    Because of = () => { ParsedDate = Parser.Parse("2009/01/21"); };

    Behaves_like<DateTimeParsingBehavior> a_date_time_parser;
  }

  [Subject("Date time parsing")]
  public class when_a_date_is_parsed_by_the_infrastructure : with_string_parser
  {
    Establish context = () => { Parser = new InfrastructureParser(); };

    Because of = () => { ParsedDate = Parser.Parse("2009/01/21"); };

    Behaves_like<DateTimeParsingBehavior> a_date_time_parser;
  }

  public abstract class with_string_parser
  {
    protected static DateTime ParsedDate;
    protected static IParser Parser;
  }

  [Behaviors]
  public class DateTimeParsingBehavior
  {
    protected static DateTime ParsedDate;

    It should_parse_the_expected_date = () => ParsedDate.ShouldEqual(new DateTime(2009, 1, 21));
  }
}
