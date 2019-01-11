using System.IO;

using Machine.Specifications.Reporting.Model;

using Spark;

namespace Machine.Specifications.Reporting.Generation.Spark
{
    public interface ISparkRenderer
    {
        void Render(Run run, TextWriter writer);
        void RenderIndex(Run run, TextWriter writer);
    }

    public class SparkRenderer : ISparkRenderer
    {
        readonly ISparkViewEngine _engine;
        readonly SparkViewDescriptor _report;
        readonly SparkViewDescriptor _index;

        public SparkRenderer()
        {
            var factory = new SparkViewEngineFactory();

            _engine = factory.CreateViewEngine();

            _report = new SparkViewDescriptor().AddTemplate(SparkViewEngineFactory.ReportTemplate);
            _index = new SparkViewDescriptor().AddTemplate(SparkViewEngineFactory.IndexTemplate);
        }

        public void Render(Run run, TextWriter writer)
        {
            var template = (SparkView)_engine.CreateInstance(_report);

            Render(template, run, writer);
        }

        public void RenderIndex(Run run, TextWriter writer)
        {
            var template = (SparkView)_engine.CreateInstance(_index);

            Render(template, run, writer);
        }

        static void Render(SparkView template, Run run, TextWriter writer)
        {
            template.Model = run;
            template.RenderView(writer);
        }
    }
}