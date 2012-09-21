using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners.Notifications;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  public class PerContextRunListener : ISpecificationRunListener
  {
    readonly RemoteTask _contextTask;
    readonly IRemoteTaskServer _server;
    readonly IList<RemoteTaskNotification> _taskNotifications = new List<RemoteTaskNotification>();

    public PerContextRunListener(IRemoteTaskServer server, RemoteTask contextNode)
    {
      _server = server;
      _contextTask = contextNode;
    }

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
      var notify = CreateTaskNotificationFor(specification);

      notify(task => _server.TaskStarting(task));
      notify(task => _server.TaskProgress(task, "Running specification"));
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      var notify = CreateTaskNotificationFor(specification);

      notify(task => _server.TaskProgress(task, null));

      notify(task => _server.TaskOutput(task, result.ConsoleOut, TaskOutputType.STDOUT));
      notify(task => _server.TaskOutput(task, result.ConsoleError, TaskOutputType.STDERR));

      TaskResult taskResult = TaskResult.Success;
      string message = null;
      switch (result.Status)
      {
        case Status.Failing:
          notify(task => _server.TaskExplain(task, result.Exception.Message));
          notify(task => _server.TaskException(task,
                                               ExceptionResultConverter.ConvertExceptions(result.Exception, out message)));
          taskResult = TaskResult.Exception;
          break;

        case Status.Passing:
          taskResult = TaskResult.Success;
          break;

        case Status.NotImplemented:
          notify(task => _server.TaskExplain(task, "Not implemented"));
          message = "Not implemented";
          taskResult = TaskResult.Skipped;
          break;

        case Status.Ignored:
          notify(task => _server.TaskExplain(task, "Ignored"));
          taskResult = TaskResult.Skipped;
          break;
      }

      notify(task => _server.TaskFinished(task, message, taskResult));
    }

    public void OnFatalError(ExceptionResult exception)
    {
      string message;
      _server.TaskExplain(_contextTask, "Fatal error: " + exception.Message);
      _server.TaskException(_contextTask, ExceptionResultConverter.ConvertExceptions(exception, out message));
      _server.TaskFinished(_contextTask, message, TaskResult.Exception);
    }

    internal void RegisterTaskNotification(RemoteTaskNotification notification)
    {
      _taskNotifications.Add(notification);
    }

    Action<Action<RemoteTask>> CreateTaskNotificationFor(SpecificationInfo specification)
    {
      return actionToBePerformedForEachTask =>
        {
          foreach (var notification in _taskNotifications)
          {
            if (notification.Matches(specification))
            {
              Debug.WriteLine(String.Format("Notifcation for {0} {1}, with {2} remote tasks",
                                            specification.ContainingType,
                                            specification.FieldName,
                                            notification.RemoteTasks.Count()));

              foreach (var task in notification.RemoteTasks)
              {
                actionToBePerformedForEachTask(task);
              }
            }
          }
        };
    }

    delegate void Action<T>(T arg);
  }
}