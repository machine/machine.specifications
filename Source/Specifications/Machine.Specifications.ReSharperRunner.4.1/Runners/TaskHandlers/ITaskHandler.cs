using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Runners.TaskHandlers
{
  internal interface ITaskHandler
  {
    bool Accepts(RemoteTask task);
    TaskResult Start(IRemoteTaskServer server, TaskExecutionNode node);
    TaskResult Execute(IRemoteTaskServer server, TaskExecutionNode node);
    TaskResult Finish(IRemoteTaskServer server, TaskExecutionNode node);
  }
}