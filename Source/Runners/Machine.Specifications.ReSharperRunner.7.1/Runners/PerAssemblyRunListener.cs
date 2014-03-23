using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners.Notifications;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  class PerAssemblyRunListener : ISpecificationRunListener
  {
    class RedirectOutput : IDisposable
    {
      readonly TextWriter _oldOut;
      readonly TextWriter _oldError;
      readonly TraceListenerCollection _oldListeners;
      internal readonly TextWriter StdOut;
      internal readonly TextWriter StdError;
      internal readonly TextWriter DebugTrace;

      public RedirectOutput()
      {
        _oldOut = Console.Out;
        _oldError = Console.Error;
        _oldListeners = Trace.Listeners;

        StdOut = new StringWriter(CultureInfo.CurrentCulture);
        Console.SetOut(StdOut);

        StdError = new StringWriter(CultureInfo.CurrentCulture);
        Console.SetError(StdError);

        DebugTrace = new StringWriter(CultureInfo.CurrentCulture);
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new TextWriterTraceListener(DebugTrace));
      }

      public void Dispose()
      {
        Console.SetOut(_oldOut);
        Console.SetError(_oldError);
        try
        {
          Trace.Listeners.Clear();
          Trace.Listeners.AddRange(_oldListeners);
        }
        catch (Exception) { }
      }
    }

    readonly RunAssemblyTask _runAssemblyTask;
    readonly IRemoteTaskServer _server;
    readonly IList<RemoteTaskNotification> _taskNotifications = new List<RemoteTaskNotification>();
    int _specifications;
    int _successes;
    int _errors;
    ContextInfo _currentContext;
    RedirectOutput _contextOutput;
    RedirectOutput _specificationOutput;

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

      _contextOutput = new RedirectOutput();

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

      NotifyRedirectedOutput(notify, _contextOutput);
      notify(task => _server.TaskFinished(task, null, result));
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      _specifications += 1;

      _specificationOutput = new RedirectOutput();

      var notify = CreateTaskNotificationFor(specification, _currentContext);
      notify(task => _server.TaskStarting(task));
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      var notify = CreateTaskNotificationFor(specification, _currentContext);

      NotifyRedirectedOutput(notify, _specificationOutput);

      var taskResult = TaskResult.Success;
      string message = null;
      switch (result.Status)
      {
        case Status.Failing:
          _errors += 1;

          notify(task => _server.TaskExplain(task, result.Exception.Message));
          notify(task => _server.TaskException(task,
                                               ExceptionResultConverter.ConvertExceptions(result.Exception, out message)));
          taskResult = TaskResult.Exception;
          break;

        case Status.Passing:
          _successes += 1;

          taskResult = TaskResult.Success;
          break;

        case Status.NotImplemented:
          notify(task => _server.TaskExplain(task, "Not implemented"));
          message = "Not implemented";
          taskResult = TaskResult.Inconclusive;
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
      _server.TaskExplain(_runAssemblyTask, "Fatal error: " + exception.Message);
      _server.TaskException(_runAssemblyTask, ExceptionResultConverter.ConvertExceptions(exception, out message));
      _server.TaskFinished(_runAssemblyTask, message, TaskResult.Exception);

      _errors += 1;
    }

    internal void RegisterTaskNotification(RemoteTaskNotification notification)
    {
      _taskNotifications.Add(notification);
    }

    Action<Action<RemoteTask>> CreateTaskNotificationFor(object infoFromRunner, ContextInfo maybeContext)
    {
      return actionToBePerformedForEachTask =>
      {
        bool invoked = false;

        foreach (var notification in _taskNotifications)
        {
          if (notification.Matches(infoFromRunner, maybeContext))
          {
            Debug.WriteLine(String.Format("Invoking notification for {0}", notification.ToString()));
            invoked = true;

            foreach (var task in notification.RemoteTasks)
            {
              actionToBePerformedForEachTask(task);
            }
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

    void NotifyRedirectedOutput(Action<Action<RemoteTask>> notify, RedirectOutput redirectOutput)
    {
      try
      {
        var output = redirectOutput;
        notify(task => _server.TaskOutput(task, output.StdOut.ToString(), TaskOutputType.STDOUT));
        notify(task => _server.TaskOutput(task, output.StdError.ToString(), TaskOutputType.STDERR));
        notify(task => _server.TaskOutput(task, output.DebugTrace.ToString(), TaskOutputType.DEBUGTRACE));
      }
      finally
      {
        redirectOutput.Dispose();
      }
    }
  }
}