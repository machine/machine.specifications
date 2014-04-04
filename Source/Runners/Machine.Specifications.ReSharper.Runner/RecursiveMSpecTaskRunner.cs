namespace Machine.Specifications.ReSharperRunner
{
    using JetBrains.ReSharper.TaskRunnerFramework;

    using Machine.Specifications.ReSharperRunner.Tasks;

    public class RecursiveMSpecTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Machine.Specifications";

        private readonly TestRunner testRunner;
        private readonly RemoteTaskServer taskServer;
        readonly IRemoteTaskServer server;

        public RecursiveMSpecTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            this.server = server;
            this.taskServer = new RemoteTaskServer(server, TaskExecutor.Configuration);
            this.testRunner = new TestRunner(this.taskServer);
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            this.taskServer.TaskRunStarting();
            this.testRunner.Run((RunAssemblyTask)node.RemoteTask, TaskProvider.Create(this.taskServer, node));
            this.taskServer.TaskRunFinished();
        }
    }
}