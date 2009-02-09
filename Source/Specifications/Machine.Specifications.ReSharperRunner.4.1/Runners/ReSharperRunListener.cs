using System;
using System.Threading;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  public class ReSharperRunListener : ISpecificationRunListener
  {
    readonly TaskExecutionNode _node;
    readonly IRemoteTaskServer _server;

    public TaskResult TaskResult
    {
      get;
      private set;
    }

    public ManualResetEvent RunFinished
    {
      get;
      private set;
    }

    public ReSharperRunListener(IRemoteTaskServer server, TaskExecutionNode node)
    {
      RunFinished = new ManualResetEvent(false);
      _server = server;
      _node = node;
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
      RunFinished.Set();
    }

    public void OnContextStart(ContextInfo context)
    {
      _server.TaskProgress(_node.RemoteTask, "Establishing context");
    }

    public void OnContextEnd(ContextInfo context)
    {
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      _server.TaskProgress(_node.RemoteTask, "Running specification");
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      _server.TaskProgress(_node.RemoteTask, String.Empty);

      _server.TaskOutput(_node.RemoteTask, result.ConsoleOut, TaskOutputType.STDOUT);
      _server.TaskOutput(_node.RemoteTask, result.ConsoleError, TaskOutputType.STDERR);

      TaskResult = TaskResult.Success;
      switch (result.Status)
      {
        case Status.Failing:
          _server.TaskExplain(_node.RemoteTask, result.Exception.Message);
          _server.TaskException(_node.RemoteTask, new[] { new TaskException(result.Exception.Exception) });
          TaskResult = TaskResult.Exception;
          break;

        case Status.Passing:
          TaskResult = TaskResult.Success;
          break;

        case Status.NotImplemented:
          _server.TaskExplain(_node.RemoteTask, "Not implemented");
          TaskResult = TaskResult.Skipped;
          break;

        case Status.Ignored:
          _server.TaskExplain(_node.RemoteTask, "Ignored");
          TaskResult = TaskResult.Skipped;
          break;
      }

      _server.TaskFinished(_node.RemoteTask, null, TaskResult);
    }

    public void OnFatalError(ExceptionResult exception)
    {
      TaskResult = TaskResult.Exception;

      _server.TaskExplain(_node.RemoteTask, "Fatal error: " + exception.Exception.Message);
      _server.TaskException(_node.RemoteTask, new[] { new TaskException(exception.Exception) });
      _server.TaskFinished(_node.RemoteTask, null, TaskResult);
    }
    #endregion
  }
}