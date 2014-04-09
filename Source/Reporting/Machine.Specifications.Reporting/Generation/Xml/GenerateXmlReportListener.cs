using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Generation.Xml
{
  public class GenerateXmlReportListener : CollectReportingInformationRunListener
  {
    readonly bool _showTimeInfo;
    readonly TimingRunListener _timer;
    readonly string _xmlPath;

    public GenerateXmlReportListener(string xmlPath, TimingRunListener timer, bool showTimeInfo)
    {
      _xmlPath = xmlPath;
      _showTimeInfo = showTimeInfo;
      _timer = timer;
    }

    protected override void OnRunEnd()
    {
      base.OnRunEnd();
      var reportGenerator = new XmlReportGenerator(_xmlPath,
                                                   ContextsByAssembly,
                                                   SpecificationsByContext,
                                                   ResultsBySpecification,
                                                   _timer,
                                                   _showTimeInfo);
      reportGenerator.WriteReports();
    }
  }
}