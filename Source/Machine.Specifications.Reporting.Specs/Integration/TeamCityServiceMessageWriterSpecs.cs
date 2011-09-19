using Machine.Specifications.Reporting.Integration;

namespace Machine.Specifications.Reporting.Specs.Integration
{
	[Subject(typeof(TeamCityServiceMessageWriter))]
	public class when_errors_are_reported
	{
		Establish context = () =>
		{
			Writer = new TeamCityServiceMessageWriter(s => Written = s);
		};

		Because of = () => Writer.WriteError("test failed", "details");

		It should_report_an_error_string =
			() => Written.ShouldEndWith("status=\'ERROR\']");

		It should_report_the_error_message =
			() => { Written.ShouldContain("test=\'test failed\'"); };
		
		It should_report_error_details =
			() => { Written.ShouldContain("errorDetails=\'details\'"); };

		static string Written;
		static TeamCityServiceMessageWriter Writer;
	}
}