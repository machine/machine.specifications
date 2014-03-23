using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Reflection;
using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners.Notifications;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  class PerAssemblyRunListener : ISpecificationRunListener
  {
    readonly RunAssemblyTask _runAssemblyTask;
    readonly IRemoteTaskServer _server;
    readonly IList<RemoteTaskNotification> _taskNotifications = new List<RemoteTaskNotification>();
    int _specifications;
    int _successes;
    int _errors;
    ContextInfo _currentContext;

    public PerAssemblyRunListener(IRemoteTaskServer server, RunAssemblyTask runAssemblyTask)
    {
      _server = server;
      _runAssemblyTask = runAssemblyTask;
    }

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      _server.TaskStarting(_runAssemblyTask);
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      var notify = CreateTaskNotificationFor(assembly, _runAssemblyTask);
      NotifyRedirectedOutput(notify, assembly);
    }

    public void OnRunStart()
    {
    }

    public void OnRunEnd()
    {
    }

    public void OnContextStart(ContextInfo context)
    {
      _specifications = 0;
      _errors = 0;
      _successes = 0;

      // TODO: This sucks, but there's no better way unless we make behaviors first-class citizens.
      _currentContext = context;
      var notify = CreateTaskNotificationFor(context, context);
      notify(task => _server.TaskStarting(task));
    }

    public void OnContextEnd(ContextInfo context)
    {
      var result = TaskResult.Inconclusive;
      if (_specifications == _successes)
      {
        result = TaskResult.Success;
      }
      if (_errors > 0)
      {
        result = TaskResult.Error;
      }

      var notify = CreateTaskNotificationFor(context, _currentContext);

      NotifyRedirectedOutput(notify, context);
      notify(task => _server.TaskFinished(task, null, result));
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      _specifications += 1;

      var notify = CreateTaskNotificationFor(specification, _currentContext);
      notify(task => _server.TaskStarting(task));
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      var notify = CreateTaskNotificationFor(specification, _currentContext);

      NotifyRedirectedOutput(notify, specification);

      var taskResult = TaskResult.Success;
      string message = null;
      switch (result.Status)
      {
        case Status.Failing:
          _errors += 1;

          notify(task => _server.TaskException(task,
                                               ExceptionResultConverter.ConvertExceptions(result.Exception, out message)));
          taskResult = TaskResult.Exception;
          break;

        case Status.Passing:
          _successes += 1;

          taskResult = TaskResult.Success;
          break;

        case Status.NotImplemented:
          notify(task => _server.TaskOutput(task, "Not implemented", TaskOutputType.STDOUT));
          message = "Not implemented";
          taskResult = TaskResult.Inconclusive;
          break;

        case Status.Ignored:
          notify(task => _server.TaskOutput(task, "Ignored", TaskOutputType.STDOUT));
          taskResult = TaskResult.Skipped;
          break;
      }

      notify(task => _server.TaskFinished(task, message, taskResult));
    }

    public void OnFatalError(ExceptionResult exception)
    {
      string message;
      _server.TaskOutput(_runAssemblyTask, "Fatal error: " + exception.Message, TaskOutputType.STDOUT);
      _server.TaskException(_runAssemblyTask, ExceptionResultConverter.ConvertExceptions(exception, out message));
      _server.TaskFinished(_runAssemblyTask, message, TaskResult.Exception);

      _errors += 1;
    }

    internal void RegisterTaskNotification(RemoteTaskNotification notification)
    {
      _taskNotifications.Add(notification);
    }

    Action<Action<RemoteTask>> CreateTaskNotificationFor(object infoFromRunner, object maybeContext)
    {
      return actionToBePerformedForEachTask =>
      {
        bool invoked = false;

        foreach (var notification in _taskNotifications)
        {
          if (!notification.Matches(infoFromRunner, maybeContext))
          {
            continue;
          }

          Debug.WriteLine(String.Format("Invoking notification for {0}", notification.ToString()));
          invoked = true;

          foreach (var task in notification.RemoteTasks)
          {
            actionToBePerformedForEachTask(task);
          }
        }

        if (!invoked)
        {
          Debug.WriteLine(String.Format("No matching notification for {0} received by the runner",
                                        infoFromRunner.ToString()));
        }
      };
    }

    delegate void Action<T>(T arg);

    void NotifyRedirectedOutput(Action<Action<RemoteTask>> notify, object maybeHasCapturedOutput)
    {
      var capture = maybeHasCapturedOutput.GetFieldOrPropertyValue("CapturedOutput");
      if (capture == null)
      {
        // Info doesn't have captured output, nothing to report.
        return;
      }

      var stdOutWithStdErrorAndDebugTrace = capture as string;
      if (stdOutWithStdErrorAndDebugTrace == null)
      {
        return;
      }

      notify(task => _server.TaskOutput(task, stdOutWithStdErrorAndDebugTrace, TaskOutputType.STDOUT));
    }
  }
}