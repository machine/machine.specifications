using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Reporting.Generation.Spark;

using Rhino.Mocks;

namespace Machine.Specifications.Reporting.Specs.Generation.Spark
{
  [Subject(typeof(GenerateSparkHtmlReportRunListener))]
  public class when_the_run_has_finished
  {
    static GenerateSparkHtmlReportRunListener Listener;
    static ISpecificationTreeReportGenerator ReportGenerator;

    Establish context = () =>
      {
        ReportGenerator = MockRepository.GenerateStub<ISpecificationTreeReportGenerator>();

        Listener = new GenerateSparkHtmlReportRunListener("C:\\path\\to\\the\\report.html", true)
                   {
                     ReportGenerator = ReportGenerator
                   };

        Listener.OnRunStart();
      };

    Because of = () => Listener.OnRunEnd();

    It should_generate_the_run_report =
      () => ReportGenerator.AssertWasCalled(x => x.GenerateReport(Listener.Run));
  }
}