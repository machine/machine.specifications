using System;
using System.Diagnostics;
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
      Debug.WriteLine("Assembly start: " + assembly.Name);
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      Debug.WriteLine("Assembly end: " + assembly.Name);
    }

    public void OnRunStart()
    {
      Debug.WriteLine("Run start");
    }

    public void OnRunEnd()
    {
      Debug.WriteLine("Run end");
      RunFinished.Set();
    }

    public void OnContextStart(ContextInfo context)
    {
      Debug.WriteLine("Context start: " + context.FullName);
    }

    public void OnContextEnd(ContextInfo context)
    {
      Debug.WriteLine("Context end: " + context.FullName);
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      Debug.WriteLine("Specification end: " + specification.Name);
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      Debug.WriteLine(String.Format("Specification end: {0} Result: {1}", specification.Name, result.Status));

      TaskResult = TaskResult.Success;
      switch (result.Status)
      {
        case Status.Failing:
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
      Debug.WriteLine("Fatal error: " + exception.Message);
    }
    #endregion
  }
}