namespace Machine.Specifications.ReSharperRunner
{
    using System;

    using JetBrains.ReSharper.TaskRunnerFramework;

    public class SimpleRemoteTaskServer : ISimpleRemoteTaskServer
    {
        private readonly IRemoteTaskServer server;

        public SimpleRemoteTaskServer(IRemoteTaskServer server)
        {
            this.server = server;
        }

        public bool ShouldContinue { get; set; }
        public void SetTempFolderPath(string path)
        {
            this.server.SetTempFolderPath(path);
        }

        public void TaskStarting(RemoteTask remoteTask)
        {
            this.server.TaskStarting(remoteTask);
        }

        public void TaskException(RemoteTask remoteTask, TaskException[] exceptions)
        {
            this.server.TaskException(remoteTask, exceptions);
        }

        public void TaskOutput(RemoteTask remoteTask, string text, TaskOutputType outputType)
        {
            this.server.TaskOutput(remoteTask, text, outputType);
        }

        public void TaskFinished(RemoteTask remoteTask, string message, TaskResult result)
        {
            this.server.TaskFinished(remoteTask, message, result);
        }

        public void TaskExplain(RemoteTask remoteTask, string explanation)
        {
            //Does not exisit in ReSharper 8.0
        }

        public void TaskDuration(RemoteTask remoteTask, TimeSpan duration)
        {
            this.server.TaskDuration(remoteTask, duration);
        }

        public void CreateDynamicElement(RemoteTask remoteTask)
        {
            this.server.CreateDynamicElement(remoteTask);
        }
    }
}