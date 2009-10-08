namespace Machine.Specifications.Reporting
{
  public class GenerateXmlReportListener : CollectReportingInformationRunListener
  {
    private readonly string _xmlPath;
    private readonly bool _showTimeInfo;

    public GenerateXmlReportListener(string xmlPath, bool showTimeInfo)
    {
      _xmlPath = xmlPath;
      _showTimeInfo = showTimeInfo;
    }

    public override void OnRunEnd()
    {
      base.OnRunEnd();
      var reportGenerator = new XmlReportGenerator(_xmlPath, ContextsByAssembly, SpecificationsByContext,
                                                   ResultsBySpecification, _showTimeInfo);
      reportGenerator.WriteReports();
    }
  }
}