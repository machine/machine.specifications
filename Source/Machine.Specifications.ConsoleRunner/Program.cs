using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Reporting.Generation.Xml;
using Machine.Specifications.Reporting.Integration;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.ConsoleRunner
{
  public class Program
  {

    [STAThread]
    [LoaderOptimization(LoaderOptimization.MultiDomain)]
    public static void Main(string[] args)
    {
      var program = new Program(new DefaultConsole());
      ExitCode exitCode = program.Run(args);

      Environment.Exit((int)exitCode);
    }

    readonly IConsole _console;

    public Program(IConsole console)
    {
      _console = console;
    }

    public ExitCode Run(string[] arguments)
    {
      ExceptionReporter reporter = new ExceptionReporter(_console);

      Options options = new Options();
      if (!options.ParseArguments(arguments))
      {
        _console.WriteLine(Resources.UsageStatement);
        return ExitCode.Failure;
      }

      List<ISpecificationRunListener> listeners = new List<ISpecificationRunListener>();

      var timingListener = new TimingRunListener();
      listeners.Add(timingListener);

      ISpecificationRunListener mainListener;
      if (options.TeamCityIntegration)
      {
        mainListener = new TeamCityReporter(_console.WriteLine, timingListener);
      }
      else
      {
        mainListener = new RunListener(_console, options.Silent, timingListener);
      }

      try
      {

        if (!String.IsNullOrEmpty(options.HtmlPath))
        {
          if (IsHtmlPathValid(options.HtmlPath))
          {
            listeners.Add(GetHtmlReportListener(options));
          }
          else
          {
            _console.WriteLine("Invalid html path:" + options.HtmlPath);
            _console.WriteLine(Resources.UsageStatement);
            return ExitCode.Failure;
          }

        }

        if (!String.IsNullOrEmpty(options.XmlPath))
        {
          if (IsHtmlPathValid(options.XmlPath))
          {
            listeners.Add(GetXmlReportListener(options));
          }
          else
          {
            _console.WriteLine("Invalid xml path:" + options.XmlPath);
            _console.WriteLine(Resources.UsageStatement);
            return ExitCode.Failure;
          }
        }

        listeners.Add(mainListener);

        if (options.AssemblyFiles.Count == 0)
        {
          _console.WriteLine(Resources.UsageStatement);
          return ExitCode.Failure;
        }

        var listener = new AggregateRunListener(listeners);

        ISpecificationRunner specificationRunner = new AppDomainRunner(listener, options.GetRunOptions());
        List<Assembly> assemblies = new List<Assembly>();
        foreach (string assemblyName in options.AssemblyFiles)
        {
          if (!File.Exists(assemblyName))
          {
            throw NewException.MissingAssembly(assemblyName);
          }

          Assembly assembly = Assembly.LoadFrom(assemblyName);
          assemblies.Add(assembly);
        }

        specificationRunner.RunAssemblies(assemblies);
      }
      catch (Exception ex)
      {
        reporter.ReportException(ex);
        return ExitCode.Error;
      }

      if (mainListener is ISpecificationResultProvider)
      {
        var errorProvider = (ISpecificationResultProvider)mainListener;
        if (errorProvider.FailureOccured)
        {
          Console.WriteLine("Generic failure occurred, no idea what this is");
          return ExitCode.Failure;
        }
      }
      return ExitCode.Success;
    }

    private static ISpecificationRunListener GetXmlReportListener(Options options)
    {
      var listener = new GenerateXmlReportListener(options.XmlPath, options.ShowTimeInformation);

      return listener;
    }

    static bool IsHtmlPathValid(string path)
    {
      return Directory.Exists(Path.GetDirectoryName(path));
    }

    public ISpecificationRunListener GetHtmlReportListener(Options options)
    {
      return new GenerateSparkHtmlReportRunListener(options.HtmlPath, options.ShowTimeInformation);
    }
  }
}