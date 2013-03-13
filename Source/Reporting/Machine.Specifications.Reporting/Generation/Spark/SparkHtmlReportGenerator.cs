using System;
using System.IO;
using System.Linq;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Generation.Spark
{
  public class SparkHtmlReportGenerator : ISpecificationTreeReportGenerator
  {
    readonly IFileSystem _fileSystem;
    readonly string _path;
    readonly ISparkRenderer _renderer;
    readonly bool _showTimeInfo;
    readonly ISpecificationVisitor[] _specificationVisitors;
    readonly Func<string, TextWriter> _streamFactory;
    Func<string> _resourcePathCreator;

    public SparkHtmlReportGenerator(string path, bool showTimeInfo)
      : this(path,
             showTimeInfo,
             new FileSystem(),
             new SparkRenderer(),
             p => new StreamWriter(p),
             new ISpecificationVisitor[]
             {
               new FailedSpecificationLinker(),
               new NotImplementedSpecificationLinker(),
               new IgnoredSpecificationLinker(),
               new FileBasedResultSupplementPreparation()
             })
    {
    }

    public SparkHtmlReportGenerator(string path,
                                    bool showTimeInfo,
                                    IFileSystem fileSystem,
                                    ISparkRenderer renderer,
                                    Func<string, TextWriter> streamFactory,
                                    ISpecificationVisitor[] specificationVisitors)
    {
      _path = path;
      _showTimeInfo = showTimeInfo;
      _fileSystem = fileSystem;
      _renderer = renderer;
      _streamFactory = streamFactory;
      _specificationVisitors = specificationVisitors;
    }

    public void GenerateReport(Run run)
    {
      run.Meta.ShouldGenerateTimeInfo = _showTimeInfo;

      Func<string, string> resourcePathCreator = p => String.Empty;
      Action<Run> writeReport = r => { };

      if (_fileSystem.IsValidPathToDirectory(_path))
      {
        resourcePathCreator = p => CreateResourceDirectoryIn(p);
        writeReport = r => WriteReportsToDirectory(r);
      }
      else if (_fileSystem.IsValidPathToFile(_path))
      {
        resourcePathCreator = p => CreateResourceDirectoryIn(Path.GetDirectoryName(p));
        writeReport = r => WriteReportToFile(r, _path);
      }

      _resourcePathCreator = () => resourcePathCreator(_path);
      writeReport(run);
    }

    void WriteReportsToDirectory(Run run)
    {
      var indexFilePath = GetIndexFilePath();

      run.Assemblies
        .Select(assembly => new Run(new[] { assembly })
                            {
                              Meta =
                                {
                                  GeneratedAt = run.Meta.GeneratedAt,
                                  ShouldGenerateTimeInfo = run.Meta.ShouldGenerateTimeInfo,
                                  ShouldGenerateIndexLink = true,
                                  IndexLink = Path.GetFileName(indexFilePath)
                                }
                            })
        .Each(assembyRun =>
          {
            var path = GetReportFilePath(assembyRun);
            WriteReportToFile(assembyRun, path);
          });

      WriteIndexToFile(run, indexFilePath);
    }

    void WriteIndexToFile(Run run, string path)
    {
      WriteReport(path, run, (renderer, writer, theRun) => renderer.RenderIndex(theRun, writer));
    }

    void WriteReportToFile(Run run, string path)
    {
      WriteReport(path, run, (renderer, writer, theRun) => renderer.Render(theRun, writer));
    }

    void WriteReport(string path, Run run, Action<ISparkRenderer, TextWriter, Run> renderAction)
    {
      _specificationVisitors.Each(x => x.Initialize(new VisitorContext { ResourcePathCreator = _resourcePathCreator }));
      _specificationVisitors.Each(x => x.Visit(run));

      _fileSystem.DeleteIfFileExists(path);

      using (var writer = _streamFactory(path))
      {
        renderAction(_renderer, writer, run);
      }
    }

    string CreateResourceDirectoryIn(string directory)
    {
      var resourcePath = Path.Combine(directory, "resources");

      _fileSystem.EnsureDirectoryExists(resourcePath);
      return resourcePath;
    }

    string GetReportFilePath(Run run)
    {
      return String.Format("{0}.html", Path.Combine(_path, run.Assemblies.First().Name));
    }

    string GetIndexFilePath()
    {
      return Path.Combine(_path, "index.html");
    }
  }
}