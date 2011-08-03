using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Runner;

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

        Listener = new GenerateSparkHtmlReportRunListener(@"C:\path\to\the\report.html", true)
                   {
                     ReportGenerator = ReportGenerator
                   };

        var assembly = new AssemblyInfo("assembly", "location");
        var context = new ContextInfo("conext", "concern", "System.Type", "System", "mscorlib");
        var specification = new SpecificationInfo("spec", "System.Type", "field");

        Listener.OnRunStart();
        Listener.OnAssemblyStart(assembly);
        Listener.OnContextStart(context);
        Listener.OnSpecificationStart(specification);
        Listener.OnSpecificationEnd(specification, Result.Pass());
        Listener.OnContextEnd(context);
        Listener.OnAssemblyEnd(assembly);
      };

    Because of = () => Listener.OnRunEnd();

    It should_generate_the_run_report =
      () => ReportGenerator.AssertWasCalled(x => x.GenerateReport(Listener.Run));
  }

  [Subject(typeof(GenerateSparkHtmlReportRunListener))]
  public class when_the_run_has_finished_without_specs_being_run
  {
    static GenerateSparkHtmlReportRunListener Listener;
    static ISpecificationTreeReportGenerator ReportGenerator;

    Establish context = () =>
    {
      ReportGenerator = MockRepository.GenerateStub<ISpecificationTreeReportGenerator>();

      Listener = new GenerateSparkHtmlReportRunListener(@"C:\path\to\the\report.html", true)
      {
        ReportGenerator = ReportGenerator
      };

      Listener.OnRunStart();
    };

    Because of = () => Listener.OnRunEnd();

    It should_not_generate_a_run_report =
      () => ReportGenerator.AssertWasNotCalled(x => x.GenerateReport(null), o => o.IgnoreArguments());
  }
}