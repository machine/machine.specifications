using System;
using System.Reflection;

using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class RunListener : ISpecificationRunListener
  {
    readonly IConsole _console;
    string _assemblyName;
    int _contextCount;
    int _specificationCount;
    int _failedSpecificationCount;

    public bool FailureOccured
    {
      get; private set;
    }

    public RunListener(IConsole console)
    {
      _console = console;
    }

    public void OnAssemblyStart(Assembly assembly)
    {
      _assemblyName = assembly.GetName().Name;
      _console.WriteLine("");
      _console.WriteLine("Specs in " + _assemblyName + ":");
      _console.WriteLine("");
    }

    public void OnAssemblyEnd(Assembly assembly)
    {
    }

    public void OnRunStart()
    {
      _contextCount = 0;
      _specificationCount = 0;
      _failedSpecificationCount = 0;
    }

    public void OnRunEnd()
    {
      var line = String.Format("Contexts: {0}, Specifications: {1}", _contextCount, _specificationCount);

      if (_failedSpecificationCount > 0)
      {
        line += String.Format(" ({0} failed)", _failedSpecificationCount);
      }

      _console.WriteLine(line);
    }

    public void OnContextStart(Context context)
    {
      _console.WriteLine(context.FullName);
    }

    public void OnContextEnd(Context context)
    {
      _console.WriteLine("");
      _contextCount += 1;
    }

    public void OnSpecificationStart(Specification specification)
    {
      _console.WriteLine("» " + specification.Name);
    }

    public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
    {
      _specificationCount += 1;
      if (!result.Passed)
      {
        _failedSpecificationCount += 1;
        FailureOccured = true;
        _console.WriteLine(result.Exception.ToString());
      }
    }
  }
}