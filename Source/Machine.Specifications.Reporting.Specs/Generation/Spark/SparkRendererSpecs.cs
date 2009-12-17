using System.IO;
using System.Linq;

using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Reporting.Model;

namespace Machine.Specifications.Reporting.Specs.Generation.Spark
{
  [Subject(typeof(SparkRenderer))]
  public class when_the_report_is_rendered
  {
    static Run Run;
    static SparkRenderer Renderer;
    static StringWriter Writer;

    Establish context = () =>
      {
        Writer = new StringWriter();
        
        Run = new Run(Enumerable.Empty<Assembly>());

        Renderer = new SparkRenderer();
      };

    Because of = () => Renderer.Render(Run, Writer);

    Cleanup after = () => Writer.Dispose();

    It should_create_a_report =
      () => Writer.ToString().ShouldNotBeEmpty();
  }
}