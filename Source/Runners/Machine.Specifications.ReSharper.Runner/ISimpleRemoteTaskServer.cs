namespace Machine.Specifications.ReSharperRunner
{
    using System;

    using JetBrains.ReSharper.TaskRunnerFramework;

    public interface ISimpleRemoteTaskServer
    {
        bool ShouldContinue { get; set; }
        void SetTempFolderPath(string path);
        void TaskStarting(RemoteTask remoteTask);
        void TaskException(RemoteTask remoteTask, TaskException[] exceptions);
        void TaskOutput(RemoteTask remoteTask, string text, TaskOutputType outputType);
        void TaskFinished(RemoteTask remoteTask, string message, TaskResult result);
        void TaskExplain(RemoteTask remoteTask, string explanation);
        void TaskDuration(RemoteTask remoteTask, TimeSpan duration);
        void CreateDynamicElement(RemoteTask remoteTask);
    }
}