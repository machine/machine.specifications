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
      string pageScreenshotPath = Path.Combine(ImagesPath, guid.ToString() + "-full-page-screenshot.png");
      string screenshotPath = Path.Combine(ImagesPath, guid.ToString() + "-screenshot.png");
      string htmlPath = Path.Combine(ImagesPath, guid.ToString() + ".html");

      Selenium.CaptureEntirePageScreenshot(pageScreenshotPath, "");
      Selenium.CaptureScreenshot(screenshotPath);

      Dictionary<string, string> supplement = new Dictionary<string, string>();

      supplement["text-log"] = Selenium.RetrieveLastRemoteControlLogs();

      using (var writer = new StreamWriter(htmlPath))
      {
        writer.Write(Selenium.GetHtmlSource());
      }

      supplement["img-full-page-screenshot"] = pageScreenshotPath;
      supplement["img-screenshot"] = screenshotPath;
      supplement["html-source"] = htmlPath;  

      return Result.Supplement(result, "selenium", supplement);
    }
  }
}
