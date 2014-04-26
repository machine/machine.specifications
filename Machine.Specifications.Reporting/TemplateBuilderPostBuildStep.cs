using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

using Machine.Specifications.Reporting.Generation.Spark;

using Spark;

namespace Machine.Specifications.Reporting
{
  [RunInstaller(true)]
  public class TemplateBuilderPostBuildStep : Installer
  {
    public override void Install(IDictionary stateSaver)
    {
      var factory = new SparkViewEngineFactory();

      var descriptors = new List<SparkViewDescriptor>
                        {
                          new SparkViewDescriptor().AddTemplate(SparkViewEngineFactory.ReportTemplate),
                          new SparkViewDescriptor().AddTemplate(SparkViewEngineFactory.IndexTemplate)
                        };

      var targetPath = factory.TemplateAssemblyPath;
      
      var engine = factory.CreateViewEngine();
      engine.BatchCompilation(targetPath, descriptors);
    }

    public override void Commit(IDictionary savedState)
    {
    }
  }
}