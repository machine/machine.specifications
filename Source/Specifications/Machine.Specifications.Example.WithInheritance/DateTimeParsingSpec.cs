using System;

// This is a rather contrived example, but I hope it shows what you can do with it.
// I like the concept of MbUnit's RowTest very much, especially when it comes to
// testing the same logic with different input patterns/files, etc.

namespace Machine.Specifications.Example.WithInheritance
{
	public class when_a_date_is_parsed_with_the_regular_expression_parser : with_string_parser
	{
		Establish context = () => { Parser = new RegexParser(); };
	}

	public class when_a_date_is_parsed_by_the_infrastructure : with_string_parser
	{
		Establish context = () => { Parser = new InfrastructureParser(); };
	}

	public abstract class with_string_parser
	{
		static DateTime ParsedDate;
		protected static IParser Parser;

		protected Because of = () => { ParsedDate = Parser.Parse("2009/01/21"); };

		protected It should_parse_the_expected_date = () => ParsedDate.ShouldEqual(new DateTime(2009, 1, 21));
	}
}