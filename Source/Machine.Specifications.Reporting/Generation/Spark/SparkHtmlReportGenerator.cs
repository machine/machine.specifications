using System;
using System.IO;
using System.Linq;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Generation.Spark
{
  public class SparkHtmlReportGenerator : ISpecificationTreeReportGenerator
  {
    readonly IFileSystem _fileSystem;
    readonly string _path;
    readonly ISparkRenderer _renderer;
    readonly Func<string, TextWriter> _streamFactory;
    string _resourcePath;

    public SparkHtmlReportGenerator(string path, bool showTimeInfo)
      : this(path, showTimeInfo, new FileSystem(), new SparkRenderer(), p => new StreamWriter(p))
    {
    }

    public SparkHtmlReportGenerator(string path,
                                    bool showTimeInfo,
                                    IFileSystem fileSystem,
                                    ISparkRenderer renderer,
                                    Func<string, TextWriter> streamFactory)
    {
      _path = path;
      _fileSystem = fileSystem;
      _renderer = renderer;
      _streamFactory = streamFactory;
    }

    public void GenerateReport(Run run)
    {
      if (_fileSystem.IsValidPathToDirectory(_path))
      {
        CreateResourceDirectoryIn(_path);
        WriteReportsToDirectory(run);
      }
      else if (_fileSystem.IsValidPathToFile(_path))
      {
        CreateResourceDirectoryIn(Path.GetDirectoryName(_path));
        WriteReportToFile(run, _path);
      }
    }

    void WriteReportsToDirectory(Run run)
    {
      run.Assemblies
        .Select(assembly => new Run(new[] { assembly })
                            {
                              GeneratedAt = run.GeneratedAt
                            })
        .Each(assembyRun =>
          {
            var path = GetReportFilePath(assembyRun);
            WriteReportToFile(assembyRun, path);
          });
    }

    void WriteReportToFile(Run run, string path)
    {
      _fileSystem.DeleteIfFileExists(path);

      using (var writer = _streamFactory(path))
      {
        _renderer.Render(run, writer);
      }
    }

    void CreateResourceDirectoryIn(string directory)
    {
      _resourcePath = Path.Combine(directory, "resources");

      _fileSystem.CreateOrOverwriteDirectory(_resourcePath);
    }

    string GetReportFilePath(Run run)
    {
      return String.Format("{0}_{1:MMddyyyy_HHmmss}.html",
                           Path.Combine(_path, run.Assemblies.First().Name),
                           run.GeneratedAt);
    }
  }
}