using System;
using System.Collections.Generic;
using System.IO;

using WatiN.Core;
using WatiN.Core.UtilityClasses;

namespace Machine.Specifications.WatinSupport
{
  /// <summary>
  /// Watin result supplementer logging browser snapshot and page html in files.
  /// </summary>
  public abstract class WatinResultSupplementer : ISupplementSpecificationResults
  {
    protected abstract string ImagesPath { get; }

    protected abstract int Quality { get; }

    protected abstract int ScalePercentage { get; }

    protected abstract bool ShowGuides { get; }

    protected virtual bool SuppressFullPageScreenshotErrors
    {
      get
      {
        return false;
      }
    }

    protected abstract Browser Watin { get; }

    protected abstract bool WriteUrl { get; }

    /// <summary>
    /// Supplements the result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public Result SupplementResult(Result result)
    {
      if (result.Status != Status.Failing)
      {
        return result;
      }

      Guid guid = Guid.NewGuid();
      string pageScreenshotPath = Path.Combine(this.ImagesPath, guid + "-full-page-screenshot.png");
      string htmlPath = Path.Combine(this.ImagesPath, guid + ".html");

      var supplement = new Dictionary<string, string>();

      try
      {
        new CaptureWebPage(this.Watin).CaptureWebPageToFile(
          pageScreenshotPath, this.WriteUrl, this.ShowGuides, this.ScalePercentage, this.Quality);
        supplement["img-full-page-screenshot"] = pageScreenshotPath;
      }
      catch (Exception err)
      {
        if (!this.SuppressFullPageScreenshotErrors)
        {
          Console.Error.WriteLine("Problem capturing Watin img-full-page-screenshot");
          Console.Error.WriteLine(err);
        }
      }

      try
      {
        using (var writer = new StreamWriter(htmlPath))
        {
          writer.Write(this.Watin.Html);
        }
        supplement["html-source"] = htmlPath;
      }
      catch (Exception err)
      {
        Console.Error.WriteLine("Problem capturing Watin html-source");
        Console.Error.WriteLine(err);
      }

      return Result.Supplement(result, "Watin", supplement);
    }
  }
}