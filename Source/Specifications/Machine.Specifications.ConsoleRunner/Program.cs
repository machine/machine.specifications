using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Reporting;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class Program
  {

    [STAThread]
    //[LoaderOptimization]
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
      ExitCode exitCode;
      ExceptionReporter reporter = new ExceptionReporter(_console);
      var runListener = new RunListener(_console);
      
      List<ISpecificationRunListener> listeners = new List<ISpecificationRunListener>();
      
      try
      {

        Options options = new Options();
        if (!options.ParseArguments(arguments))
        {
          _console.WriteLine(Resources.UsageStatement);
          return ExitCode.Failure;
        }

        if (!IsHtmlPathUnspecifiedOrSpecifiedAndValid(options, listeners))
        {
          _console.WriteLine("Invalid html path:" + options.HtmlPath);
          _console.WriteLine(Resources.UsageStatement);
          return ExitCode.Failure;
        }

        if (!options.Silent)
          listeners.Add(runListener);
        
        if (options.AssemblyFiles.Count == 0)
        {
          _console.WriteLine(Resources.UsageStatement);
          return ExitCode.Failure;
        }

        var listener = new AggregateRunListener(listeners);
        
        ISpecificationRunner specificationRunner = new AppDomainRunner(listener);
        foreach (string assemblyName in options.AssemblyFiles)
        {
          if (!File.Exists(assemblyName))
          {
            throw NewException.MissingAssembly(assemblyName);
          }

          Assembly assembly = Assembly.LoadFrom(assemblyName);
          specificationRunner.RunAssembly(assembly);
        }
      }
      catch(Exception ex)
      {
        reporter.ReportException(ex);
        return ExitCode.Error;
      }

      if (runListener.FailureOccured)
      {
        return ExitCode.Failure;
      }

      return ExitCode.Success;
    }

    public bool IsHtmlPathUnspecifiedOrSpecifiedAndValid(Options options, List<ISpecificationRunListener> listeners)
    {
      if (!options.HtmlPath.Equals(string.Empty))
      {
        var reportingListener = new GenerateHtmlReportRunListener(options.HtmlPath, options.ShowTimeInformation);
        listeners.Add(reportingListener);
        return true;
      }
      return true;
    }
  }
}