using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  public class PerContextRunListener : ISpecificationRunListener
  {
    readonly RemoteTask _contextTask;
    readonly IRemoteTaskServer _server;
    readonly IList<ExecutableSpecificationInfo> _specifications = new List<ExecutableSpecificationInfo>();

    public PerContextRunListener(IRemoteTaskServer server, RemoteTask contextNode)
    {
      _server = server;
      _contextTask = contextNode;
    }

    #region Implementation of ISpecificationRunListener
    public void OnAssemblyStart(AssemblyInfo assembly)
    {
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
    }

    public void OnRunStart()
    {
    }

    public void OnRunEnd()
    {
    }

    public void OnContextStart(ContextInfo context)
    {
      _server.TaskProgress(_contextTask, "Running context");
    }

    public void OnContextEnd(ContextInfo context)
    {
      _server.TaskProgress(_contextTask, null);
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      RemoteTask task = FindTaskFor(specification);
      if (task == null)
      {
        return;
      }

      _server.TaskStarting(task);
      _server.TaskProgress(task, "Running specification");
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      RemoteTask task = FindTaskFor(specification);
      if (task == null)
      {
        return;
      }

      _server.TaskProgress(task, null);

      _server.TaskOutput(task, result.ConsoleOut, TaskOutputType.STDOUT);
      _server.TaskOutput(task, result.ConsoleError, TaskOutputType.STDERR);

      TaskResult taskResult = TaskResult.Success;
      switch (result.Status)
      {
        case Status.Failing:
          _server.TaskExplain(task, result.Exception.Message);
          _server.TaskException(task, new[] { new TaskException(result.Exception.ToFakeException()) });
          taskResult = TaskResult.Exception;
          break;

        case Status.Passing:
          taskResult = TaskResult.Success;
          break;

        case Status.NotImplemented:
          _server.TaskExplain(task, "Not implemented");
          taskResult = TaskResult.Skipped;
          break;

        case Status.Ignored:
          _server.TaskExplain(task, "Ignored");
          taskResult = TaskResult.Skipped;
          break;
      }

      _server.TaskFinished(task, null, taskResult);
    }

    public void OnFatalError(ExceptionResult exception)
    {
      _server.TaskExplain(_contextTask, "Fatal error: " + exception.Message);
      _server.TaskException(_contextTask, new[] { new TaskException(exception.ToFakeException()) });
      _server.TaskFinished(_contextTask, null, TaskResult.Exception);
    }
    #endregion

    internal void RegisterSpecification(ExecutableSpecificationInfo info)
    {
      _specifications.Add(info);
    }

    RemoteTask FindTaskFor(SpecificationInfo specification)
    {
      var knownSpecification = _specifications.FirstOrDefault(x => x.ContainingType == specification.ContainingType &&
                                                                   x.Name == specification.Name);

      if (knownSpecification == null)
      {
        return null;
      }
      return knownSpecification.RemoteTask;
    }
  }

  public static class ExceptionExtensions
  {
    public static Exception ToFakeException(this ExceptionResult exceptionResult)
    {
      return new FakeException(exceptionResult);
    }
  }

  [Serializable]
  public class FakeException : Exception
  {
    readonly ExceptionResult _exceptionResult;
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public FakeException(ExceptionResult exceptionResult) : base(exceptionResult.Message)
    {
      _exceptionResult = exceptionResult;
    }

    public FakeException(string message) : base(message)
    {
    }

    public FakeException(string message, Exception inner) : base(message, inner)
    {

    }

    public override string StackTrace
    {
      get
      {
        return _exceptionResult.StackTrace;
      }
    }

    public override string Message
    {
      get
      {
        return _exceptionResult.Message;
      }
    }

    public override string ToString()
    {
      return _exceptionResult.ToString();
    }

    protected FakeException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}