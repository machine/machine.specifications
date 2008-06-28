using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class Program
  {
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
      var listener = new RunListener(_console);

      try
      {
        if (arguments.Length == 0)
        {
          _console.WriteLine(Resources.UsageStatement);
          return ExitCode.Failure;
        }

        SpecificationRunner specificationRunner = new SpecificationRunner(listener);
        foreach (string assemblyName in arguments)
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

      if (listener.FailureOccured)
      {
        return ExitCode.Failure;
      }

      return ExitCode.Success;
    }
  }
}