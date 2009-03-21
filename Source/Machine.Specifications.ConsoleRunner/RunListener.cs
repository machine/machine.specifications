using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class RunListener : ISpecificationRunListener, ISpecificationResultProvider
  {
    readonly IConsole _console;
    readonly bool _silent;
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

    public RunListener(IConsole console, bool silent)
    {
      _console = console;
      _silent = silent;
    }

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      _currentAssemblyName = assembly.Name;
      WriteLineVerbose("");
      WriteLineVerbose("Specs in " + _currentAssemblyName + ":");
      WriteLineVerbose("");
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
      WriteLineVerbose(context.FullName);
    }

    public void OnContextEnd(ContextInfo context)
    {
      WriteVerbose("");
      _contextCount += 1;
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      WriteVerbose("» " + specification.Name);
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      _specificationCount += 1;
      switch(result.Status)
      {
        case Status.Passing:
          _passedSpecificationCount += 1;
          WriteLineVerbose("");
          break;
        case Status.NotImplemented:
          _unimplementedSpecificationCount += 1;
          WriteLineVerbose(" (NOT IMPLEMENTED)");
          break;
        case Status.Ignored:
          _ignoredSpecificationCount += 1;
          WriteLineVerbose(" (IGNORED)");
          break;
        default:
          _failedSpecificationCount += 1;
          FailureOccured = true;
          WriteLineVerbose(" (FAIL)");
          WriteLineVerbose(result.Exception.ToString());
          break;
      }
    }

    public void OnFatalError(ExceptionResult exception)
    {
      FailureOccured = true;
      _console.WriteLine("Fatal Error");
      _console.WriteLine(exception.ToString());
    }

    void WriteVerbose(string str)
    {
      if (!_silent) _console.Write(str);
    }

    void WriteLineVerbose(string str)
    {
      if (!_silent) _console.WriteLine(str);
    }
  }
}