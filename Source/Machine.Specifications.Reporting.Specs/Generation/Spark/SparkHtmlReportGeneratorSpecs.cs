using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

using Rhino.Mocks;

namespace Machine.Specifications.Reporting.Specs.Generation.Spark
{
  [Subject(typeof(SparkHtmlReportGenerator))]
  public class when_reporting_to_a_file
  {
    static SparkHtmlReportGenerator Generator;
    static Run Run;
    static IFileSystem FileSystem;
    static string ReportPath;
    static string ResourceDirectory;
    static ISparkRenderer Renderer;
    static string ReportPathUsed;

    Establish context = () =>
      {
        ReportPath = @"C:\path\to\the\report.html";
        ResourceDirectory = @"C:\path\to\the\resources";

        FileSystem = MockRepository.GenerateStub<IFileSystem>();
        FileSystem
          .Stub(x => x.IsValidPathToDirectory(ReportPath))
          .Return(false);
        FileSystem
          .Stub(x => x.IsValidPathToFile(ReportPath))
          .Return(true);

        Renderer = MockRepository.GenerateStub<ISparkRenderer>();

        Generator = new SparkHtmlReportGenerator(ReportPath,
                                                 false,
                                                 FileSystem,
                                                 Renderer,
                                                 p =>
                                                   {
                                                     ReportPathUsed = p;
                                                     return new StringWriter();
                                                   },
                                                 new ISpecificationVisitor[] { });

        Run = new Run(new[]
                      {
                        new Assembly("assembly 1", new Concern[] { }),
                        new Assembly("assembly 2", new Concern[] { })
                      });
      };

    Because of = () => Generator.GenerateReport(Run);

    It should_create_a_directory_for_the_report_resources =
      () => FileSystem.AssertWasCalled(x => x.CreateOrOverwriteDirectory(ResourceDirectory));

    It should_overwrite_existing_reports =
      () => FileSystem.AssertWasCalled(x => x.DeleteIfFileExists(Arg<string>.Is.Anything));

    It should_create_a_report_file_as_specified =
      () => ReportPathUsed.ShouldEqual(ReportPath);

    It should_render_one_report =
      () => Renderer.AssertWasCalled(x => x.Render(Arg<Run>.Is.Equal(Run), Arg<TextWriter>.Is.NotNull));

    It should_not_render_the_report_summary =
      () => Renderer.AssertWasNotCalled(x => x.RenderIndex(Arg<Run>.Is.Anything, Arg<TextWriter>.Is.Anything));

    It should_not_link_the_report_to_the_summary =
      () => Renderer.AssertWasCalled(x => x.Render(Arg<Run>.Matches(y => y.Meta.ShouldGenerateIndexLink == false),
                                                   Arg<TextWriter>.Is.Anything));
  }

  [Subject(typeof(SparkHtmlReportGenerator))]
  public class when_reporting_to_a_directory
  {
    static SparkHtmlReportGenerator Generator;
    static Run Run;
    static IFileSystem FileSystem;
    static string ReportDirectory;
    static string ResourceDirectory;

    static ISparkRenderer Renderer;
    static IList<string> ReportPathsUsed;

    Establish context = () =>
      {
        ReportDirectory = @"C:\path\to\the\report";
        ResourceDirectory = @"C:\path\to\the\report\resources";

        FileSystem = MockRepository.GenerateStub<IFileSystem>();
        FileSystem
          .Stub(x => x.IsValidPathToDirectory(ReportDirectory))
          .Return(true);

        Renderer = MockRepository.GenerateStub<ISparkRenderer>();

        ReportPathsUsed = new List<string>();

        Generator = new SparkHtmlReportGenerator(ReportDirectory,
                                                 false,
                                                 FileSystem,
                                                 Renderer,
                                                 p =>
                                                   {
                                                     ReportPathsUsed.Add(p);
                                                     return new StringWriter();
                                                   },
                                                 new ISpecificationVisitor[] { });

        Run = new Run(new[]
                      {
                        new Assembly("assembly 1", new Concern[] { }),
                        new Assembly("assembly 2", new Concern[] { })
                      })
              {
                Meta = { GeneratedAt = new DateTime(2009, 12, 17, 19, 59, 33) }
              };
      };

    Because of = () => Generator.GenerateReport(Run);

    It should_create_a_directory_for_the_report_resources =
      () => FileSystem.AssertWasCalled(x => x.CreateOrOverwriteDirectory(ResourceDirectory));

    It should_overwrite_existing_reports_and_summaries =
      () => FileSystem.AssertWasCalled(x => x.DeleteIfFileExists(Arg<string>.Is.Anything),
                                       o => o.Repeat.Times(Run.Assemblies.Count() + 1));

    It should_create_a_reports_in_the_report_directory =
      () => ReportPathsUsed.Each(x => x.ShouldStartWith(ReportDirectory));

    It should_render_one_report_for_every_assembly_that_was_run =
      () => Renderer.AssertWasCalled(x => x.Render(Arg<Run>.Matches(y => y.Assemblies.Count() == 1),
                                                   Arg<TextWriter>.Is.NotNull),
                                     o => o.Repeat.Times(Run.Assemblies.Count()));

    It should_link_the_assembly_reports_to_the_summary =
      () => Renderer.AssertWasCalled(x => x.Render(Arg<Run>.Matches(y => y.Meta.ShouldGenerateIndexLink &&
                                                                         y.Meta.IndexLink == "index.html"),
                                                   Arg<TextWriter>.Is.Anything),
                                     o => o.Repeat.Times(Run.Assemblies.Count()));

    It should_render_the_report_summary =
      () => Renderer.AssertWasCalled(x => x.RenderIndex(Arg<Run>.Is.Same(Run), Arg<TextWriter>.Is.NotNull));

    It should_not_link_the_summary_to_itself =
      () => Renderer.AssertWasCalled(x => x.RenderIndex(Arg<Run>.Matches(y => y.Meta.ShouldGenerateIndexLink == false),
                                                        Arg<TextWriter>.Is.Anything));
  }

  [Subject(typeof(SparkHtmlReportGenerator))]
  public class when_rendering_reports_with_time_info
  {
    static SparkHtmlReportGenerator Generator;
    static Run Run;
    static IFileSystem FileSystem;
    static string ReportPath;
    static ISparkRenderer Renderer;

    Establish context = () =>
      {
        ReportPath = @"C:\path\to\the\report.html";

        FileSystem = MockRepository.GenerateStub<IFileSystem>();
        FileSystem
          .Stub(x => x.IsValidPathToFile(ReportPath))
          .Return(true);

        Renderer = MockRepository.GenerateStub<ISparkRenderer>();

        Generator = new SparkHtmlReportGenerator(ReportPath,
                                                 true,
                                                 FileSystem,
                                                 Renderer,
                                                 p => new StringWriter(),
                                                 new ISpecificationVisitor[] { });

        Run = new Run(new[]
                      {
                        new Assembly("assembly 1", new Concern[] { }),
                        new Assembly("assembly 2", new Concern[] { })
                      });
      };

    Because of = () => Generator.GenerateReport(Run);

    It should_render_the_report_with_time_info =
      () => Renderer.AssertWasCalled(x => x.Render(Arg<Run>.Matches(y => y.Meta.ShouldGenerateTimeInfo),
                                                   Arg<TextWriter>.Is.Anything));
  }

  [Subject(typeof(SparkHtmlReportGenerator))]
  public class when_rendering_reports_with_enriched_metadata_to_a_file
  {
    static SparkHtmlReportGenerator Generator;
    static Run Run;
    static IFileSystem FileSystem;
    static string ReportPath;
    static ISparkRenderer Renderer;
    static ISpecificationVisitor[] Visitors;

    Establish context = () =>
      {
        ReportPath = @"C:\path\to\the\report.html";

        FileSystem = MockRepository.GenerateStub<IFileSystem>();
        FileSystem
          .Stub(x => x.IsValidPathToFile(ReportPath))
          .Return(true);

        Renderer = MockRepository.GenerateStub<ISparkRenderer>();

        Visitors = new[]
                   {
                     MockRepository.GenerateStub<ISpecificationVisitor>(),
                     MockRepository.GenerateStub<ISpecificationVisitor>()
                   };

        Generator = new SparkHtmlReportGenerator(ReportPath,
                                                 true,
                                                 FileSystem,
                                                 Renderer,
                                                 p => new StringWriter(),
                                                 Visitors);

        Run = new Run(new[]
                      {
                        new Assembly("assembly 1", new Concern[] { }),
                        new Assembly("assembly 2", new Concern[] { })
                      });
      };

    Because of = () => Generator.GenerateReport(Run);

    It should_initialize_all_visitors =
      () => Visitors.Each(v => v.AssertWasCalled(x => x.Initialize(Arg<VisitorContext>.Is.NotNull)));

    It should_enrich_the_report_using_all_visitors =
      () => Visitors.Each(v => v.AssertWasCalled(x => x.Visit(Run)));
  }

  [Subject(typeof(SparkHtmlReportGenerator))]
  public class when_rendering_reports_with_enriched_metadata_to_a_directory
  {
    static SparkHtmlReportGenerator Generator;
    static Run Run;
    static IFileSystem FileSystem;
    static string ReportPath;
    static ISparkRenderer Renderer;
    static ISpecificationVisitor[] Visitors;

    Establish context = () =>
      {
        ReportPath = @"C:\path\to\the\report.html";

        FileSystem = MockRepository.GenerateStub<IFileSystem>();
        FileSystem
          .Stub(x => x.IsValidPathToFile(ReportPath))
          .Return(false);
        FileSystem
          .Stub(x => x.IsValidPathToDirectory(ReportPath))
          .Return(true);

        Renderer = MockRepository.GenerateStub<ISparkRenderer>();

        Visitors = new[]
                   {
                     MockRepository.GenerateStub<ISpecificationVisitor>(),
                     MockRepository.GenerateStub<ISpecificationVisitor>()
                   };

        Generator = new SparkHtmlReportGenerator(ReportPath,
                                                 true,
                                                 FileSystem,
                                                 Renderer,
                                                 p => new StringWriter(),
                                                 Visitors);

        Run = new Run(new[]
                      {
                        new Assembly("assembly 1", new Concern[] { }),
                        new Assembly("assembly 2", new Concern[] { })
                      });
      };

    Because of = () => Generator.GenerateReport(Run);

    It should_initialize_all_visitors_for_each_assembly_report_and_the_index =
      () => Visitors.Each(v => v.AssertWasCalled(x => x.Initialize(Arg<VisitorContext>.Is.NotNull),
                                                 o => o.Repeat.Times(Run.TotalAssemblies + 1)));

    It should_enrich_all_reports_and_the_index_using_all_visitors =
      () => Visitors.Each(v => v.AssertWasCalled(x => x.Visit(Arg<Run>.Is.NotNull),
                                                 o => o.Repeat.Times(Run.TotalAssemblies + 1)));
  }
}
