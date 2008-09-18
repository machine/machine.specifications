using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class RunListener : ISpecificationRunListener
  {
    readonly IConsole _console;
    string _currentAssemblyName;
    int _contextCount;
    int _specificationCount;
    int _failedSpecificationCount;
    int _unimplementedSpecificationCount;
    int _passedSpecificationCount;

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
      _currentAssemblyName = assembly.GetName().Name;
      _console.WriteLine("");
      _console.WriteLine("Specs in " + _currentAssemblyName + ":");
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
      _unimplementedSpecificationCount = 0;
      _passedSpecificationCount = 0;
    }

    public void OnRunEnd()
    {
      var line = String.Format("Contexts: {0}, Specifications: {1}", _contextCount, _specificationCount);
      
      if (_failedSpecificationCount > 0 || _unimplementedSpecificationCount > 0)
      {
        line += String.Format("\n  {0} passed, {1} failed", _passedSpecificationCount,_failedSpecificationCount);
        if (_unimplementedSpecificationCount > 0)
          line += String.Format(", {0} unimplemented", _unimplementedSpecificationCount);
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
      switch(result.Result)
      {
        case Result.Passed:
          _passedSpecificationCount += 1;
          break;
        case Result.Unimplemented:
          _unimplementedSpecificationCount += 1;
          _console.WriteLine("   ...unimplemented");
          break;
        default:
          _failedSpecificationCount += 1;
          FailureOccured = true;
          _console.WriteLine(result.Exception.ToString());
          break;
      }
    }
  }
}