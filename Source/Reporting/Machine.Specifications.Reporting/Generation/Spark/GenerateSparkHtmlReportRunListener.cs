namespace Machine.Specifications.Reporting.Generation.Spark
{
  public class GenerateSparkHtmlReportRunListener : SpecificationTreeListener
  {
    public GenerateSparkHtmlReportRunListener(string htmlPath, bool showTimeInfo)
    {
      ReportGenerator = new SparkHtmlReportGenerator(htmlPath, showTimeInfo);
    }

    public ISpecificationTreeReportGenerator ReportGenerator
    {
      get;
      set;
    }

    public override void OnRunEnd()
    {
      base.OnRunEnd();

      if(Run.TotalContexts == 0)
      {
        return;
      }

      ReportGenerator.GenerateReport(Run);
    }
  }
}