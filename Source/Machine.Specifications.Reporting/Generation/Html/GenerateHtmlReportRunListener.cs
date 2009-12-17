namespace Machine.Specifications.Reporting.Generation.Html
{
  public class GenerateHtmlReportRunListener : CollectReportingInformationRunListener
  {

    string _htmlPath;
    bool _showTimeInfo;

    public GenerateHtmlReportRunListener(string htmlPath, bool showTimeInfo)
      : base()
    {
      _htmlPath = htmlPath;
      _showTimeInfo = showTimeInfo;
    }

    public override void OnRunEnd()
    {
      base.OnRunEnd();
      HtmlReportGenerator reportGenerator = new HtmlReportGenerator(_htmlPath, ContextsByAssembly, SpecificationsByContext, ResultsBySpecification, _showTimeInfo);
      reportGenerator.WriteReports();
    }
  }
}


