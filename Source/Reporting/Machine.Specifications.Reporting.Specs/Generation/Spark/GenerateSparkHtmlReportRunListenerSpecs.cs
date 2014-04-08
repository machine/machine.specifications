using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Runner.Utility;

using Rhino.Mocks;

namespace Machine.Specifications.Reporting.Specs.Generation.Spark
{
    [Subject(typeof(GenerateSparkHtmlReportRunListener))]
    public class when_the_run_has_finished
    {
        static TestableGenerateSparkHtmlReportRunListener Listener;
        static ISpecificationTreeReportGenerator ReportGenerator;

        Establish context = () =>
          {
              ReportGenerator = MockRepository.GenerateStub<ISpecificationTreeReportGenerator>();

              Listener = new TestableGenerateSparkHtmlReportRunListener(@"C:\path\to\the\report.html", true)
                         {
                             ReportGenerator = ReportGenerator
                         };

              var assembly = new AssemblyInfo("assembly", "location");
              var context = new ContextInfo("conext", "concern", "System.Type", "System", "mscorlib");
              var specification = new SpecificationInfo("it", "spec", "System.Type", "field");

              Listener.TestOnRunStart();
              Listener.TestOnAssemblyStart(assembly);
              Listener.TestOnContextStart(context);
              Listener.TestOnSpecificationStart(specification);
              Listener.TestOnSpecificationEnd(specification, Runner.Utility.Result.Pass());
              Listener.TestOnContextEnd(context);
              Listener.TestOnAssemblyEnd(assembly);
          };

        Because of = () => Listener.TestOnRunEnd();

        It should_generate_the_run_report =
          () => ReportGenerator.AssertWasCalled(x => x.GenerateReport(Listener.Run));
    }

    [Subject(typeof(GenerateSparkHtmlReportRunListener))]
    public class when_the_run_has_finished_without_specs_being_run
    {
        static TestableGenerateSparkHtmlReportRunListener Listener;
        static ISpecificationTreeReportGenerator ReportGenerator;

        Establish context = () =>
        {
            ReportGenerator = MockRepository.GenerateStub<ISpecificationTreeReportGenerator>();

            Listener = new TestableGenerateSparkHtmlReportRunListener(@"C:\path\to\the\report.html", true)
            {
                ReportGenerator = ReportGenerator
            };

            Listener.TestOnRunStart();
        };

        Because of = () => Listener.TestOnRunEnd();

        It should_not_generate_a_run_report =
          () => ReportGenerator.AssertWasNotCalled(x => x.GenerateReport(null), o => o.IgnoreArguments());
    }

    internal class TestableGenerateSparkHtmlReportRunListener : GenerateSparkHtmlReportRunListener
    {
        public TestableGenerateSparkHtmlReportRunListener(string htmlPath, bool showTimeInfo) : base(htmlPath, showTimeInfo)
        {
        }

        public void TestOnRunStart()
        {
            this.OnRunStart();
        }

        public void TestOnRunEnd()
        {
            this.OnRunEnd();
        }

        public void TestOnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            this.OnAssemblyStart(assemblyInfo);
        }

        public void TestOnContextStart(ContextInfo context)
        {
            this.OnContextStart(context);
        }

        public void TestOnSpecificationStart(SpecificationInfo specification)
        {
            this.OnSpecificationStart(specification);
        }

        public void TestOnSpecificationEnd(SpecificationInfo specification, Runner.Utility.Result result)
        {
            this.OnSpecificationEnd(specification, result);
        }

        public void TestOnContextEnd(ContextInfo context)
        {
            this.OnContextEnd(context);
        }

        public void TestOnAssemblyEnd(AssemblyInfo assembly)
        {
            this.OnAssemblyEnd(assembly);
        }
    }
}