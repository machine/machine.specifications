using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Specifications.ConsoleRunner.Properties;
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
    int _ignoredSpecificationCount;
    int _passedSpecificationCount;

    public bool FailureOccured
    {
      get; private set;
    }

    public RunListener(IConsole console)
    {
      _console = console;
    }

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      _currentAssemblyName = assembly.Name;
      _console.WriteLine("");
      _console.WriteLine("Specs in " + _currentAssemblyName + ":");
      _console.WriteLine("");
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
    }

    public void OnRunStart()
    {
      _contextCount = 0;
      _specificationCount = 0;
      _failedSpecificationCount = 0;
      _unimplementedSpecificationCount = 0;
      _ignoredSpecificationCount = 0;
      _passedSpecificationCount = 0;
    }

    public void OnRunEnd()
    {
      var line = String.Format("Contexts: {0}, Specifications: {1}", _contextCount, _specificationCount);
      
      if (_failedSpecificationCount > 0 || _unimplementedSpecificationCount > 0)
      {
        line += String.Format("\n  {0} passed, {1} failed", _passedSpecificationCount,_failedSpecificationCount);
        if (_unimplementedSpecificationCount > 0)
        {
          line += String.Format(", {0} not implemented", _unimplementedSpecificationCount);
        }
        if (_ignoredSpecificationCount > 0)
        {
          line += String.Format(", {0} ignored", _ignoredSpecificationCount);
        }
      }
      _console.WriteLine(line);
    }

    public void OnContextStart(ContextInfo context)
    {
      _console.WriteLine(context.FullName);
    }

    public void OnContextEnd(ContextInfo context)
    {
      _console.WriteLine("");
      _contextCount += 1;
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      _console.Write("» " + specification.Name);
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      _specificationCount += 1;
      switch(result.Status)
      {
        case Status.Passing:
          _passedSpecificationCount += 1;
          _console.WriteLine("");
          break;
        case Status.NotImplemented:
          _unimplementedSpecificationCount += 1;
          _console.WriteLine(" (NOT IMPLEMENTED)");
          break;
        case Status.Ignored:
          _ignoredSpecificationCount += 1;
          _console.WriteLine(" (IGNORED)");
          break;
        default:
          _failedSpecificationCount += 1;
          FailureOccured = true;
          _console.WriteLine(" (FAIL)");
          _console.WriteLine(result.Exception.ToString());
          break;
      }
    }

    public void OnFatalError(ExceptionResult exception)
    {
      _console.WriteLine("Fatal Error");
      _console.WriteLine(exception.ToString());
    }
  }
}