using System.IO;

using Machine.Specifications.Reporting.Model;

using Spark;

using Assembly = System.Reflection.Assembly;

namespace Machine.Specifications.Reporting.Generation.Spark
{
  public interface ISparkRenderer
  {
    void Render(Run run, TextWriter writer);
  }

  public class SparkRenderer : ISparkRenderer
  {
    readonly ISparkViewEngine _engine;
    readonly SparkViewDescriptor _descriptor;

    public SparkRenderer()
    {
      var factory = new SparkViewEngineFactory();
      
      _engine = factory.CreateViewEngine();
      _engine.LoadBatchCompilation(Assembly.Load(factory.TemplateAssembly));
      
      _descriptor = new SparkViewDescriptor().AddTemplate(SparkViewEngineFactory.DefaultTemplate);
    }

    public void Render(Run run, TextWriter writer)
    {
      var template = (SparkView) _engine.CreateInstance(_descriptor);

      template.Model = run;
      template.RenderView(writer);
    }
  }
}