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

        FileSystem = MockRepository.GenerateStub<IFileSystem>();
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

    It should_overwrite_existing_reports =
      () => FileSystem.AssertWasCalled(x => x.DeleteIfFileExists(Arg<string>.Is.Anything),
                                       o => o.Repeat.Times(Run.Assemblies.Count()));

    It should_create_a_reports_in_the_report_directory =
      () => ReportPathsUsed.Each(x => x.ShouldStartWith(ReportDirectory));

    It should_put_the_report_generation_date_in_the_file_name =
      () => ReportPathsUsed.Each(x => x.ShouldEndWith("_12172009_195933.html"));

    It should_render_one_report_for_every_assembly_that_was_run =
      () => Renderer.AssertWasCalled(x => x.Render(Arg<Run>.Matches(y => y.Assemblies.Count() == 1),
                                                   Arg<TextWriter>.Is.NotNull),
                                     o => o.Repeat.Times(Run.Assemblies.Count()));
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
  public class when_rendering_reports_with_enriched_metadata
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

    It should_enrich_the_report_using_all_visitors =
      () => Visitors.Each(v => v.AssertWasCalled(x => x.Visit(Run)));
  }
}
