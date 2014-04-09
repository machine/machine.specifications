using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner
{
    using System;
    using System.Collections.Generic;

    using JetBrains.ReSharper.TaskRunnerFramework;

    using Machine.Specifications.ReSharperRunner.Tasks;

    public class PerAssemblyRunListener : SpecificationRunListenerBase
    {
        private readonly RemoteTaskServer _server;
        private readonly TaskProvider _taskProvider;

        //using a stack to make sure, that node tree or tasks are finished in reverse order (first in last out)
        //example: if we pass a contex task and start it, then passing a spec and start it, 
        //we have to make sure that the spec is finishd first
        private readonly Stack<TaskState> _states = new Stack<TaskState>();
        private readonly HashSet<RemoteTask> runTests = new HashSet<RemoteTask>();
        private bool _hasInconclusives;

        private class TaskState
        {
            public readonly RemoteTask _Task;
            public TaskResult _Result;
            public string _Message;
            public TimeSpan _Duration;

            public TaskState(RemoteTask task, string message = "Internal Error (mspec runner): No status reported", TaskResult result = TaskResult.Inconclusive)
            {
                this._Task = task;
                this._Message = message;
                this._Result = result;
            }

            public void SetPassed()
            {
                this._Result = TaskResult.Success;
                this._Message = string.Empty;
            }
        }

        private TaskState CurrentState { get { return this._states.Peek(); } }

        public PerAssemblyRunListener(RemoteTaskServer server, TaskProvider taskProvider)
        {
            this._taskProvider = taskProvider;
            this._server = server;
        }

        protected override void OnAssemblyStart(AssemblyInfo remoteAssembly)
        {
        }

        protected override void OnAssemblyEnd(AssemblyInfo remoteAssembly)
        {
        }

        protected override void OnRunStart()
        {
        }

        protected override void OnRunEnd()
        {
        }

        //Force resharper to start remoteContext task. In the meantime, resharper UI lets rotate a green ball 
        //next to the remoteContext node to signalize that tests has been started
        protected override void OnContextStart(ContextInfo remoteContext)
        {
            this._states.Push(new TaskState(this._taskProvider.GetContextTask(remoteContext.TypeName), string.Empty, TaskResult.Success));
            this._server.TaskStarting(this.CurrentState._Task);
        }

        //Context task finished. Resharper UI tree marks remoteContext as green (pass) or red (failed) etc. 
        //Context tasks finishes after all Specifications has been finished first (thats why we use a stack)
        protected override void OnContextEnd(ContextInfo remoteContext)
        {
            var state = this._states.Pop();
            if (this._hasInconclusives)
            {
                state._Result = TaskResult.Inconclusive;
                this._hasInconclusives = false;
            }
            this._server.TaskFinished(state._Task, state._Message, state._Result, state._Duration);
        }

        //Starts spec task and lets rotate a green ball next to the spec node
        protected override void OnSpecificationStart(SpecificationInfo remoteSpecification)
        {
            var specTask = this._taskProvider.GetSpecificationTask(remoteSpecification.ContainingType, remoteSpecification.FieldName);
            this.FinishPreviousSpecTaskIfStillRunning(specTask);
            this.StartNewSpecTaskIfNotAlreadyRunning(specTask);
            //StartNewTheoryTaskIfRequired
        }

        //finishes remoteContext task and marks spec in the UI tree
        //public void OnSpecificationEnd(string remoteSpecification, string remoteResult)
        protected override void OnSpecificationEnd(SpecificationInfo specification, Result remoteResult)
        {
            var state = this._states.Pop();

            switch (remoteResult.Status)
            {
                case Status.Failing:
                    state._Result = TaskResult.Error;
                    ExceptionResult exception;
                    if (remoteResult.Exception.InnerExceptionResult != null)
                    {
                        state._Message = remoteResult.Exception.InnerExceptionResult.Message;
                        exception = remoteResult.Exception.InnerExceptionResult;
                    }
                    else
                    {
                        state._Message = remoteResult.Exception.Message;
                        exception = remoteResult.Exception;
                    }

                    this._server.TaskException(state._Task, ExceptionResultConverter.ConvertExceptions(exception, out state._Message));
                    break;

                case Status.Passing:
                    state.SetPassed();
                    break;

                case Status.NotImplemented:
                    //notify(task => _server.TaskOutput(task, "Not implemented", TaskOutputType.STDOUT));
                    this._server.TaskOutput(state._Task, "Not implemented", TaskOutputType.STDOUT);
                    state._Result = TaskResult.Inconclusive;
                    this._hasInconclusives = true;
                    break;

                case Status.Ignored:
                    //notify(task => _server.TaskOutput(task, "Not implemented", TaskOutputType.STDOUT));
                    this._server.TaskOutput(state._Task, "Ignored", TaskOutputType.STDOUT);
                    state._Result = TaskResult.Skipped;
                    break;
            }

            this._server.TaskFinished(state._Task, state._Message, state._Result);
        }

        protected override void OnFatalError(ExceptionResult exception)
        {
            string message = "Fatal error: " + exception.Message;

            var state = this._states.Pop();
            this._server.TaskException(state._Task, ExceptionResultConverter.ConvertExceptions(exception, out state._Message));
            this._server.TaskFinished(state._Task, message, TaskResult.Exception);
        }

        private void FinishPreviousSpecTaskIfStillRunning(RemoteTask specTask)
        {
            if (!Equals(specTask, this.CurrentState._Task) && this.CurrentState._Task is ContextSpecificationTask)
                this.OnSpecificationEnd(null, null);
        }

        private void StartNewSpecTaskIfNotAlreadyRunning(RemoteTask specTask)
        {
            if (!Equals(specTask, this.CurrentState._Task))
            {
                this._states.Push(new TaskState(specTask, string.Empty, TaskResult.Success));
                this._server.TaskStarting(specTask);
            }
        }
    }
}