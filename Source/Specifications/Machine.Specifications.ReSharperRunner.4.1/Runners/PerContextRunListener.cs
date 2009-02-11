using System.Collections.Generic;
using System.Linq;

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

      TaskResult = TaskResult.Success;
      switch (result.Status)
      {
        case Status.Failing:
          _server.TaskExplain(task, result.Exception.Message);
          _server.TaskException(task, new[] { new TaskException(result.Exception.Exception) });
          TaskResult = TaskResult.Exception;
          break;

        case Status.Passing:
          TaskResult = TaskResult.Success;
          break;

        case Status.NotImplemented:
          _server.TaskExplain(task, "Not implemented");
          TaskResult = TaskResult.Skipped;
          break;

        case Status.Ignored:
          _server.TaskExplain(task, "Ignored");
          TaskResult = TaskResult.Skipped;
          break;
      }

      _server.TaskFinished(task, null, TaskResult);
    }

    public void OnFatalError(ExceptionResult exception)
    {
      TaskResult = TaskResult.Exception;

      _server.TaskExplain(_contextTask, "Fatal error: " + exception.Exception.Message);
      _server.TaskException(_contextTask, new[] { new TaskException(exception.Exception) });
      _server.TaskFinished(_contextTask, null, TaskResult);
    }
    #endregion

    // TODO: Delete
    public TaskResult TaskResult
    {
      get;
      private set;
    }

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
}