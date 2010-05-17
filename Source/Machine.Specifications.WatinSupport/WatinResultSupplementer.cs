using System;
using System.Collections.Generic;
using System.IO;
using WatiN.Core;
using WatiN.Core.UtilityClasses;

namespace Machine.Specifications.WatinSupport
{
    public abstract class WatinResultSupplementer : ISupplementSpecificationResults
    {
        protected abstract string ImagesPath { get; }
        protected abstract Browser Watin { get; }
        protected virtual bool SuppressFullPageScreenshotErrors { get { return false; } }

        public Result SupplementResult(Result result)
        {
            if (result.Status != Status.Failing)
                return result;

            //if (result.Supplements.ContainsKey("Watin")) return result;

            var guid = Guid.NewGuid();
            var pageScreenshotPath = Path.Combine(ImagesPath, guid + "-full-page-screenshot.png");
            var htmlPath = Path.Combine(ImagesPath, guid + ".html");

            var supplement = new Dictionary<string, string>();

            try
            {
                //todo gives options to user through usage of abstarct prop
                new CaptureWebPage(this.Watin).CaptureWebPageToFile(pageScreenshotPath, false, false, 100, 100);
                supplement["img-full-page-screenshot"] = pageScreenshotPath;
            }
            catch (Exception err)
            {
                if (!SuppressFullPageScreenshotErrors)
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
