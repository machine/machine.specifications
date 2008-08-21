using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Reporting
{
  public class GenerateHtmlReportRunListener : CollectReportingInformationRunListener
  {

    string _htmlPath;

    public GenerateHtmlReportRunListener(string htmlPath)
      : base()
    {
      _htmlPath = htmlPath;
    }

    public override void OnRunEnd()
    {
      base.OnRunEnd();
      ReportGenerator reportGenerator = new ReportGenerator(this._htmlPath,this.ContextsByAssembly, this.SpecificationsByContext,this.ResultsBySpecification);

      reportGenerator.WriteReports();
    }

  }
}
