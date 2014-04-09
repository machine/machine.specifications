namespace Machine.Specifications.ReSharperRunner
{
    using System;

    using JetBrains.ReSharper.TaskRunnerFramework;

    public partial class RemoteTaskServer
    {
        private readonly ISimpleRemoteTaskServer server;
        private readonly ISimpleClientController clientController;
        private readonly ISetTempFolderPathStrategy setTempFolderPathStrategy;

        public RemoteTaskServer(IRemoteTaskServer server, TaskExecutorConfiguration configuration)
        {
            this.server = SimpleRemoteTaskServerFactory.Create(server);
            this.Configuration = configuration;
            this.clientController = SimpleClientControllerFactory.Create(server);
            this.setTempFolderPathStrategy = SetTempFolderPathStrategyFactory.Create(server);

            this.ShouldContinue = true;
        }

        public TaskExecutorConfiguration Configuration { get; private set; }

        public bool ShouldContinue
        {
            get { return this.server.ShouldContinue; }
            set { this.server.ShouldContinue = value; }
        }

        public void SetTempFolderPath(string path)
        {
            this.setTempFolderPathStrategy.SetTempFolderPath(path);
        }

        public void TaskRunStarting()
        {
            this.clientController.TaskRunStarting();
        }

        public void TaskStarting(RemoteTask remoteTask)
        {
            this.clientController.TaskStarting(remoteTask);
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
            this.TaskFinished(remoteTask, message, result, TimeSpan.Zero);
        }

        public void TaskFinished(RemoteTask remoteTask, string message, TaskResult result, TimeSpan duration)
        {

            this.clientController.TaskFinished(remoteTask);
            if (result == TaskResult.Skipped)
                this.server.TaskExplain(remoteTask, message);
            if (duration != TimeSpan.Zero)
                this.server.TaskDuration(remoteTask, duration);
            this.server.TaskFinished(remoteTask, message, result);
        }

        public void TaskRunFinished()
        {
            this.clientController.TaskRunFinished();
            this.setTempFolderPathStrategy.TestRunFinished();
        }

        public void CreateDynamicElement(RemoteTask remoteTask)
        {
            this.server.CreateDynamicElement(remoteTask);
        }
    }
}
