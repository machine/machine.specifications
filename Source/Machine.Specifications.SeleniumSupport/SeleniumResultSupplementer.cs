using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Selenium;

namespace Machine.Specifications.SeleniumSupport
{
  public abstract class SeleniumResultSupplementer : ISupplementSpecificationResults
  {
    protected abstract string ImagesPath { get; }
    protected abstract DefaultSelenium Selenium { get; }

    public Result SupplementResult(Result result)
    {
      if (result.Passed)
        return result;

      Guid guid = Guid.NewGuid();
      string pageScreenshotPath = Path.Combine(ImagesPath, guid.ToString() + "-full-page-screenshot");
      string screenshotPath = Path.Combine(ImagesPath, guid.ToString() + "-screenshot");

      Selenium.CaptureEntirePageScreenshot(pageScreenshotPath, "");
      Selenium.CaptureScreenshot(screenshotPath);

      Dictionary<string, string> supplement = new Dictionary<string, string>();

      supplement["full-page-screenshot"] = pageScreenshotPath;
      supplement["screenshot"] = screenshotPath;
      supplement["html"] = Selenium.GetHtmlSource();
      supplement["log"] = Selenium.RetrieveLastRemoteControlLogs();

      return Result.Supplement(result, "selenium", supplement);
    }
  }
}
