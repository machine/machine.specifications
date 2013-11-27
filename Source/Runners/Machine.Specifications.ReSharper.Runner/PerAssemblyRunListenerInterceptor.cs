using System;
using System.Collections.Generic;
using System.Diagnostics;

using Castle.DynamicProxy;

using JetBrains.Reflection;
using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners;
using Machine.Specifications.ReSharperRunner.Runners.Notifications;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharper.Runner
{
    class PerAssemblyRunListenerInterceptor : StandardInterceptor // ISpecificationRunListener
    {
        readonly RunAssemblyTask _runAssemblyTask;
        readonly IRemoteTaskServer _server;
        readonly IList<RemoteTaskNotification> _taskNotifications = new List<RemoteTaskNotification>();
        int _specifications;
        int _successes;
        int _errors;
        ContextInfo _currentContext;

        public PerAssemblyRunListenerInterceptor(IRemoteTaskServer server, RunAssemblyTask runAssemblyTask)
        {
            _server = server;
            _runAssemblyTask = runAssemblyTask;
        }

        // TODO: There have been some signature changes through the ages
        // We might get called with different types (although unlikely, and it was 5 years ago)
        // OnSpecificationEnd(..., Result) was SpecificationResult (27/09/2008 - 4e0f9a08d4bde7f3b7045e9cf503a73ff9406ad1)
        // AssemblyInfo was Assembly, ContextInfo was Model.Context, SpecificationInfo was Model.Specification (23/09/2008 - aa9621b707503a0d7bdaec6870aad5a952fee6c3)
        protected override void PerformProceed(IInvocation invocation)
        {
            switch (invocation.Method.Name)
            {
                case "OnAssemblyStart":
                    OnAssemblyStart();
                    break;

                case "OnAssemblyEnd":
                    OnAssemblyEnd(new AssemblyInfo(invocation.Arguments[0]));
                    break;

                case "OnContextStart":
                    OnContextStart(new ContextInfo(invocation.Arguments[0]));
                    break;

                case "OnContextEnd":
                    OnContextEnd(new ContextInfo(invocation.Arguments[0]));
                    break;

                case "OnSpecificationStart":
                    OnSpecificationStart(new SpecificationInfo(invocation.Arguments[0]));
                    break;

                case "OnSpecificationEnd":
                    OnSpecificationEnd(new SpecificationInfo(invocation.Arguments[0]), new Result(invocation.Arguments[1]));
                    break;

                case "OnFatalError":
                    OnFatalError(new ExceptionResult(invocation.Arguments[0]));
                    break;
            }
        }

        void OnAssemblyStart()
        {
            _server.TaskStarting(_runAssemblyTask);
        }

        void OnAssemblyEnd(AssemblyInfo assembly)
        {
            var notify = CreateTaskNotificationFor(assembly, _runAssemblyTask);
            NotifyRedirectedOutput(notify, assembly);
        }

        void OnContextStart(ContextInfo context)
        {
            _specifications = 0;
            _errors = 0;
            _successes = 0;

            // TODO: This sucks, but there's no better way unless we make behaviors first-class citizens.
            _currentContext = context;
            var notify = CreateTaskNotificationFor(context, context);
            notify(task => _server.TaskStarting(task));
        }

        void OnContextEnd(ContextInfo context)
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

        void OnSpecificationStart(SpecificationInfo specification)
        {
            _specifications += 1;

            var notify = CreateTaskNotificationFor(specification, _currentContext);
            notify(task => _server.TaskStarting(task));
        }

        void OnSpecificationEnd(SpecificationInfo specification, Result result)
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

        void OnFatalError(ExceptionResult exception)
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