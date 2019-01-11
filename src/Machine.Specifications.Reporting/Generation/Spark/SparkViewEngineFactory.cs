using System.Collections.Generic;
using System.IO;
using System.Linq;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Runner.Utility;
using Spark;
using Spark.FileSystem;

namespace Machine.Specifications.Reporting.Generation.Spark
{
    public class SparkViewEngineFactory
    {
        public const string ReportTemplate = "report.spark";
        public const string IndexTemplate = "index.spark";

        public ISparkViewEngine CreateViewEngine()
        {
            var settings = new SparkSettings()
                .AddViewFolder(ViewFolderType.EmbeddedResource, new Dictionary<string, string>
                {
                    {"assembly", typeof(SparkView).Assembly.FullName},
                    {"resourcePath", "Machine.Specifications.Reporting.Generation.Spark.Templates"}
                })
                .SetPageBaseType(typeof(SparkView))
                .SetAutomaticEncoding(true)
                .AddNamespace(typeof(string).Namespace)
                .AddNamespace(typeof(Enumerable).Namespace)
                .AddNamespace(typeof(Status).Namespace)
                .AddNamespace(typeof(Run).Namespace);

            return new SparkViewEngine(settings);
        }
    }
}