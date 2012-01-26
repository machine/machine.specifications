using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

using OpenQA.Selenium;

namespace Machine.Specifications.WebDriverSupport
{
  public abstract class WebDriverResultSupplementer : ISupplementSpecificationResults
  {
    protected abstract string ImagesPath { get; }
    protected abstract ITakesScreenshot Screenshotter { get; }
    protected abstract IWebDriver WebDriver { get; }

    public Result SupplementResult(Result result)
    {
      if (result.Status != Status.Failing)
      {
        return result;
      }

      var guid = Guid.NewGuid();
      var pageScreenshotPath = Path.Combine(ImagesPath, guid + "-full-page-screenshot.png");
      var htmlPath = Path.Combine(ImagesPath, guid + ".html");

      var supplement = new Dictionary<string, string>();

      CaptureScreenshot(supplement, pageScreenshotPath);
      CaptureHtmlSource(supplement, htmlPath);

      return Result.Supplement(result, "WebDriver", supplement);
    }

    void CaptureScreenshot(IDictionary<string, string> supplement, string screenshotPath)
    {
      try
      {
        Screenshotter.GetScreenshot().SaveAsFile(screenshotPath, ImageFormat.Png);
        supplement["img-full-page-screenshot"] = screenshotPath;
      }
      catch (Exception error)
      {
        Report(error, "img-full-page-screenshot", supplement);
      }
    }

    void CaptureHtmlSource(IDictionary<string, string> supplement, string htmlPath)
    {
      try
      {
        using (var writer = new StreamWriter(htmlPath))
        {
          writer.Write(WebDriver.PageSource);
        }
        supplement["html-source"] = htmlPath;
      }
      catch (Exception error)
      {
        Report(error, "html-source", supplement);
      }
    }

    static void Report(Exception error, string type, IDictionary<string, string> supplement)
    {
      supplement[type] = error.ToString();

      Console.Error.WriteLine();
      Console.Error.WriteLine("Problem capturing WebDriver {0}", type);
      Console.Error.WriteLine(error);
    }
  }
}