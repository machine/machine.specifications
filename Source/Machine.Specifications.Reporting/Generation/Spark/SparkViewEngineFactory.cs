using System.IO;
using System.Linq;
using System.Reflection;

using Spark;
using Spark.FileSystem;

namespace Machine.Specifications.Reporting.Generation.Spark
{
  public class SparkViewEngineFactory
  {
    public const string DefaultTemplate = "master.spark";

    public string TemplateAssemblyPath
    {
      get
      {
        return Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), TemplateAssembly + ".dll");
      }
    }
    
    public string TemplateAssembly
    {
      get
      {
        return GetType().Assembly.GetName().Name + ".Templates";
      }
    }

    public ISparkViewEngine CreateViewEngine()
    {
      var settings = new SparkSettings()
        .SetPageBaseType(typeof(SparkView))
        .SetAutomaticEncoding(true)
        .AddNamespace(typeof(string).Namespace)
        .AddNamespace(typeof(Enumerable).Namespace)
        .AddNamespace(typeof(Status).Namespace);

      var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var templates = new FileSystemViewFolder(Path.Combine(dir, "Generation\\Spark\\Templates"));

      return new SparkViewEngine(settings)
             {
               ViewFolder = templates
             };
    }
  }
}