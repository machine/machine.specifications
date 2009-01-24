using System;

namespace Machine.Specifications.Specs
{
  public class when_a_date_is_parsed_with_the_regular_expression_parser : with_string_parser
  {
    Establish context = () => { Parser = new RegexParser(); };

    It_should_behave_like a_date_time_parser = () => new DateTimeParsingBehavior();
  }

  public class when_a_date_is_parsed_by_the_infrastructure : with_string_parser
  {
    Establish context = () => { Parser = new InfrastructureParser(); };

    It_should_behave_like a_date_time_parser = () => new DateTimeParsingBehavior();
    It_should_behave_like a_second = () => new SecondaryBehavior();
  }

  public abstract class with_string_parser
  {
    protected static DateTime ParsedDate;
    protected static IParser Parser;

    protected Because of = () => { ParsedDate = Parser.Parse("2009/01/21"); };
  }

  public class DateTimeParsingBehavior
  {
    protected static DateTime ParsedDate;

    It should_parse_the_expected_date = () => ParsedDate.ShouldEqual(new DateTime(2009, 1, 21));

    It is_not_implemented;

    [Ignore]
    It is_ignored;
  }
  
  public class SecondaryBehavior
  {
    protected static DateTime ParsedDate;

    It should_parse_the_expected_date_second = () => ParsedDate.ShouldEqual(new DateTime(2009, 1, 21));
    It_should_behave_like a_third = () => new NestedThirdBehavior();
  }
  
  public class NestedThirdBehavior
  {
    protected static DateTime ParsedDate;

    It should_parse_the_expected_date_nested_third = () => ParsedDate.ShouldEqual(new DateTime(2009, 1, 21));

    It is_not_implemented_nested_third;

    [Ignore]
    It is_ignored_nested_third;
  }

  #region Parsers
  public interface IParser
  {
    DateTime Parse(string date);
  }

  internal class RegexParser : IParser
  {
    #region IParser Members
    public DateTime Parse(string date)
    {
      // Parse with a regular expression. Not that it's recommended, but that's why this example is contrived.
      return new DateTime(2009, 1, 21);
    }
    #endregion
  }

  internal class InfrastructureParser : IParser
  {
    #region IParser Members
    public DateTime Parse(string date)
    {
      // Parse with DateTime.Parse.
      return new DateTime(2009, 1, 21);
    }
    #endregion
  }
  #endregion
}