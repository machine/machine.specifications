using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Reporting
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
      ReportGenerator reportGenerator = new ReportGenerator(_htmlPath, ContextsByAssembly, SpecificationsByContext, ResultsBySpecification, _showTimeInfo);
      reportGenerator.WriteReports();
    }

  }
}
