using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Reporting.Model;

namespace Machine.Specifications.Reporting.Specs.Generation.Spark
{
  [Subject(typeof(SparkRenderer))]
  public class when_an_empty_report_is_rendered
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

  [Subject(typeof(SparkRenderer))]
  public class when_a_report_with_unordered_items_is_rendered
  {
    static Run Run;
    static SparkRenderer Renderer;
    static StringWriter Writer;
    static string Html;

    Establish context = () =>
      {
        Writer = new StringWriter();

        Run = new Run(new[]
                      {
                        Assembly("assembly 2",
                                 Concern("a 2 concern 2",
                                         Context("a 2 c 2 context 2",
                                                 Spec("a 2 c 2 c 2 specification 2", Result.Pass()),
                                                 Spec("a 2 c 2 c 2 specification 1", Result.Ignored())
                                           ),
                                         Context("a 2 c 2 context 1",
                                                 Spec("a 2 c 2 c 1 specification 2",
                                                      Result.Failure(PrepareException())),
                                                 Spec("a 2 c 2 c 1 specification 1", Result.NotImplemented())
                                           )
                                   ),
                                 Concern("a 2 concern 1",
                                         Context("a 2 c 1 context 2",
                                                 Spec("a 2 c 1 c 2 specification 2",
                                                      Result.Supplement(Result.Pass(),
                                                                        "the supplement",
                                                                        new Dictionary<string, string>
                                                                        {
                                                                          { "text-some", "some <em>text</em>" },
                                                                          { "img-some", @"C:\some\image\file" },
                                                                          { "html-some", @"C:\some\html\file" }
                                                                        })),
                                                 Spec("a 2 c 1 c 2 specification 1", Result.Pass())
                                           ),
                                         Context("a 2 c 1 context 1",
                                                 Spec("a 2 c 1 c 1 specification 2", Result.Pass()),
                                                 Spec("a 2 c 1 c 1 specification 1", Result.Pass())
                                           )
                                   )),
                        Assembly("assembly 1",
                                 Concern("a 1 concern 2",
                                         Context("a 1 c 2 context 2",
                                                 Spec("a 1 c 2 c 2 specification 2", Result.Pass()),
                                                 Spec("a 1 c 2 c 2 specification 1", Result.Pass())
                                           ),
                                         Context("a 1 c 2 context 1",
                                                 Spec("a 1 c 2 c 1 specification 2", Result.Pass()),
                                                 Spec("a 1 c 2 c 1 specification 1", Result.Pass())
                                           )
                                   ),
                                 Concern("a 1 concern 1",
                                         Context("a 1 c 1 context 2",
                                                 Spec("a 1 c 1 c 2 specification 2", Result.Pass()),
                                                 Spec("a 1 c 1 c 2 specification 1", Result.Pass())
                                           ),
                                         Context("a 1 c 1 context 1",
                                                 Spec("a 1 c 1 c 1 specification 2", Result.Pass()),
                                                 Spec("a 1 c 1 c 1 specification 1", Result.Pass())
                                           )
                                   )
                          )
                      });

        Renderer = new SparkRenderer();
      };

    Because of = () =>
      {
        Renderer.Render(Run, Writer);
        Html = Writer.ToString();
      };

    Cleanup after = () => Writer.Dispose();

    It should_create_a_report =
      () => Html.ShouldNotBeEmpty();

    It should_order_assemblies_by_assembly_name =
      () =>
        {
          var assembly1 = Html.IndexOf("assembly 1");
          var assembly2 = Html.IndexOf("assembly 2");

          assembly1.ShouldBeLessThan(assembly2);
        };

    It should_order_concerns_by_name =
      () =>
        {
          var assembly1Concern1 = Html.IndexOf("a 1 concern 1");
          var assembly2Concern1 = Html.IndexOf("a 2 concern 1");

          assembly1Concern1.ShouldBeLessThan(assembly2Concern1);
        };

    It should_order_concerns_inside_assemblies_by_name =
      () =>
        {
          var assembly1Concern1 = Html.IndexOf("a 1 concern 1");
          var assembly1Concern2 = Html.IndexOf("a 1 concern 2");

          assembly1Concern1.ShouldBeLessThan(assembly1Concern2);
        };

    It should_order_contexts_by_name =
      () =>
        {
          var context1 = Html.IndexOf("a 1 c 1 context 1");
          var context2 = Html.IndexOf("a 1 c 1 context 2");

          context1.ShouldBeLessThan(context2);
        };

    It should_order_specifications_in_the_order_they_were_executed =
      () =>
        {
          var spec1 = Html.IndexOf("a 1 c 1 c 1 specification 2");
          var spec2 = Html.IndexOf("a 1 c 1 c 1 specification 1");

          spec1.ShouldBeLessThan(spec2);
        };

    static Assembly Assembly(string name, params Concern[] concerns)
    {
      return new Assembly(name, concerns);
    }

    static Concern Concern(string name, params Context[] contexts)
    {
      return new Concern(name, contexts);
    }

    static Context Context(string name, params Specification[] specifications)
    {
      return new Context(name, specifications);
    }

    static Specification Spec(string name, Result result)
    {
      return new Specification(name, result);
    }

    static Exception PrepareException()
    {
      try
      {
        try
        {
          SomeAction();
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException("something bad happened", ex);
        }
      }
      catch (Exception ex)
      {
        return ex;
      }

      return null;
    }

    static void SomeAction()
    {
      throw new NotImplementedException();
    }
  }
}