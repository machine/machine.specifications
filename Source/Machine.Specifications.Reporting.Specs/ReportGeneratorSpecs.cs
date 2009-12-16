using System;
using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Runner;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Specs
{
  [Subject(typeof(ReportGenerator))]
  public class when_rendering_the_HTML_report : ReportGeneratorSpecs
  {
    static ReportGenerator Report;
    static RunResult RunResult;
    static string Html;

    Establish context = () =>
      {
        RunResult = GenerateRunResult(result =>
          {
            result.AssemblyName = number => "assembly " + number;
            result.ConcernName = number => " concern " + number;
            result.ContextName = number => " context " + number;
            result.SpecificationName = number => " specification " + number;

            return result
              .Assemblies(4)
              .WithConcerns(2)
              .EachHavingContexts(3)
              .WithSpecifications(5)
              .OfWhichAreFailing(3)
              .Build();
          });

        Report = new ReportGenerator("",
                                     RunResult.ContextsByAssembly,
                                     RunResult.SpecificationsByContext,
                                     RunResult.ResultsBySpecification,
                                     false);
      };

    Because of = () => { Html = Report.Render(RunResult.ContextsByAssembly); };

    It should_generate_a_report =
      () => Html.ShouldNotBeNull();

    It should_order_the_assembly_names_in_the_overview =
      () =>
        {
          var assembly1 = Html.IndexOf("<h1>" + RunResult.AssemblyName(1));
          var assembly2 = Html.IndexOf("<h1>" + RunResult.AssemblyName(2));
          assembly1.ShouldBeLessThan(assembly2);
        };

    It should_order_the_assembly_concerns_by_name =
      () =>
        {
          var assemblyConcern1 = Html.IndexOf(string.Format("<h2 class=\"concern\">{0}", RunResult.AssemblyName(1)));
          var assemblyConcern2 = Html.IndexOf(string.Format("<h2 class=\"concern\">{0}", RunResult.AssemblyName(2)));
          assemblyConcern1.ShouldBeLessThan(assemblyConcern2);
        };

    It should_order_the_concerns_inside_an_assembly_by_name =
      () =>
        {
          var concern1 = Html.IndexOf(string.Format("<h2 class=\"concern\">{0}{1}",
                                                    RunResult.AssemblyName(1),
                                                    RunResult.ConcernName(1)));
          var concern2 = Html.IndexOf(string.Format("<h2 class=\"concern\">{0}{1}",
                                                    RunResult.AssemblyName(1),
                                                    RunResult.ConcernName(2)));
          concern1.ShouldBeLessThan(concern2);
        };

    It should_order_the_contexts_inside_a_concern_by_name =
      () =>
        {
          var context1 = Html.IndexOf(string.Format("<h3 class=\"context\">{0}{1}{2}",
                                                    RunResult.AssemblyName(1),
                                                    RunResult.ConcernName(1),
                                                    RunResult.ContextName(1)));
          var context2 = Html.IndexOf(string.Format("<h3 class=\"context\">{0}{1}{2}",
                                                    RunResult.AssemblyName(1),
                                                    RunResult.ConcernName(1),
                                                    RunResult.ContextName(2)));
          context1.ShouldBeLessThan(context2);
        };

    It should_arrange_the_specifications_inside_a_context_in_the_order_they_were_executed =
      () =>
        {
          var spec1 = Html.IndexOf(string.Format("<li>{0}{1}{2}{3}",
                                                 RunResult.AssemblyName(1),
                                                 RunResult.ConcernName(1),
                                                 RunResult.ContextName(1),
                                                 RunResult.SpecificationName(1)));
          var spec2 = Html.IndexOf(string.Format("<li>{0}{1}{2}{3}",
                                                 RunResult.AssemblyName(1),
                                                 RunResult.ConcernName(1),
                                                 RunResult.ContextName(1),
                                                 RunResult.SpecificationName(2)));
          spec2.ShouldBeLessThan(spec1);
        };
  }

  public class ReportGeneratorSpecs
  {
    protected static RunResult GenerateRunResult(Func<RunResult, RunResult> generator)
    {
      return generator(new RunResult());
    }
  }

  public class RunResult
  {
    public RunResult()
    {
      ContextsByAssembly = new Dictionary<string, List<ContextInfo>>();
      SpecificationsByContext = new Dictionary<ContextInfo, List<SpecificationInfo>>();
      ResultsBySpecification = new Dictionary<SpecificationInfo, Result>();
    }

    public Dictionary<string, List<ContextInfo>> ContextsByAssembly
    {
      get;
      set;
    }

    public Dictionary<ContextInfo, List<SpecificationInfo>> SpecificationsByContext
    {
      get;
      set;
    }

    public Dictionary<SpecificationInfo, Result> ResultsBySpecification
    {
      get;
      set;
    }

    public Func<int, string> AssemblyName
    {
      get;
      set;
    }

    public Func<int, string> ConcernName
    {
      get;
      set;
    }

    public Func<int, string> ContextName
    {
      get;
      set;
    }

    public Func<int, string> SpecificationName
    {
      get;
      set;
    }
  }

  internal static class RunResultExtensions
  {
    static int NumberOfAssemblies;
    static int NumberOfConcerns;
    static int NumberOfContexts;
    static int NumberOfFailingSpecs;
    static int NumberOfSpecsPerContext;

    public static RunResult Assemblies(this RunResult result, int numberOfAssemblies)
    {
      NumberOfAssemblies = numberOfAssemblies;
      return result;
    }

    public static RunResult WithConcerns(this RunResult result, int numberOfConcerns)
    {
      NumberOfConcerns = numberOfConcerns;
      return result;
    }

    public static RunResult EachHavingContexts(this RunResult result, int numberOfContexts)
    {
      NumberOfContexts = numberOfContexts;
      return result;
    }

    public static RunResult WithSpecifications(this RunResult result, int numberOfSpecs)
    {
      NumberOfSpecsPerContext = numberOfSpecs;
      return result;
    }

    public static RunResult OfWhichAreFailing(this RunResult result, int numberOfFailingSpecs)
    {
      NumberOfFailingSpecs = numberOfFailingSpecs;
      return result;
    }

    public static RunResult Build(this RunResult result)
    {
      Enumerable
        .Range(1, NumberOfAssemblies)
        .Reverse()
        .Each(x => result.ContextsByAssembly.Add(result.AssemblyName(x), new List<ContextInfo>()));

      result
        .ContextsByAssembly
        .Keys
        .Reverse()
        .Each(assembly => Enumerable
                            .Range(1, NumberOfConcerns)
                            .Reverse()
                            .Select(x => assembly + result.ConcernName(x))
                            .Each(concern => Enumerable
                                               .Range(1, NumberOfContexts)
                                               .Reverse()
                                               .Each(x =>
                                                 {
                                                   var context = new ContextInfo(concern + result.ContextName(x),
                                                                                 concern,
                                                                                 "typeName",
                                                                                 "namespace",
                                                                                 "assemblyName");
                                                   result.ContextsByAssembly[assembly].Add(context);

                                                   result.SpecificationsByContext.Add(context,
                                                                                      new List<SpecificationInfo>());
                                                 })));

      Action<int, ContextInfo, Result> specificationGenerator = (x, context, status) =>
        {
          var specification = new SpecificationInfo(context.Name + result.SpecificationName(x), context.TypeName);
          result.SpecificationsByContext[context].Add(specification);

          result.ResultsBySpecification.Add(specification, status);
        };

      result
        .SpecificationsByContext
        .Keys
        .Reverse()
        .Each(context =>
          {
            Enumerable
              .Range(1, NumberOfSpecsPerContext - NumberOfFailingSpecs)
              .Reverse()
              .Each(x => specificationGenerator(x, context, Result.Pass()));

            Enumerable
              .Range(NumberOfSpecsPerContext - NumberOfFailingSpecs + 1, NumberOfFailingSpecs)
              .Reverse()
              .Each(x => specificationGenerator(x, context, Result.Failure(new Exception())));
          });

      return result;
    }
  }
}