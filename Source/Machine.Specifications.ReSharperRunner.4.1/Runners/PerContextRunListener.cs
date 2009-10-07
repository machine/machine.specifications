using System.Collections.Generic;

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
      string message = null;
      switch (result.Status)
      {
        case Status.Failing:
          _server.TaskExplain(task, result.Exception.Message);
          _server.TaskException(task, ExceptionResultConverter.ConvertExceptions(result.Exception, out message));
          taskResult = TaskResult.Exception;
          break;

        case Status.Passing:
          taskResult = TaskResult.Success;
          break;

        case Status.NotImplemented:
          _server.TaskExplain(task, "Not implemented");
          message = "Not implemented";
          taskResult = TaskResult.Skipped;
          break;

        case Status.Ignored:
          _server.TaskExplain(task, "Ignored");
          taskResult = TaskResult.Skipped;
          break;
      }

	  _server.TaskFinished(task, message, taskResult);
    }

    public void OnFatalError(ExceptionResult exception)
    {
      string message;
      _server.TaskExplain(_contextTask, "Fatal error: " + exception.Message);
      _server.TaskException(_contextTask, ExceptionResultConverter.ConvertExceptions(exception, out message));
      _server.TaskFinished(_contextTask, message, TaskResult.Exception);
    }
    #endregion

    internal void RegisterSpecification(ExecutableSpecificationInfo info)
    {
      _specifications.Add(info);
    }

    RemoteTask FindTaskFor(SpecificationInfo specification)
    {
      foreach(var spec in _specifications)
      {
        if (spec.ContainingType == specification.ContainingType &&
            spec.Name == specification.Name)
        {
          return spec.RemoteTask;
        }
      }

      return null;
    }
  }
}
