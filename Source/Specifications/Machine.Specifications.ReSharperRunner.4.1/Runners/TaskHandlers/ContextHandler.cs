using System.Diagnostics;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Runners.TaskHandlers
{
  internal class ContextHandler : ITaskHandler
  {
    #region Implementation of ITaskHandler<ContextTask>
    public bool Accepts(RemoteTask task)
    {
      return task is ContextTask;
    }

    public TaskResult Start(IRemoteTaskServer server, TaskExecutionNode node)
    {
      ContextTask task = (ContextTask) node.RemoteTask;
      Debug.WriteLine("Start context: " + task.ContextTypeName);

      return TaskResult.Success;
    }

    public TaskResult Execute(IRemoteTaskServer server, TaskExecutionNode node)
    {
      ContextTask task = (ContextTask) node.RemoteTask;
      Debug.WriteLine("Execute context: " + task.ContextTypeName);

      return TaskResult.Success;
    }

    public TaskResult Finish(IRemoteTaskServer server, TaskExecutionNode node)
    {
      ContextTask task = (ContextTask) node.RemoteTask;
      Debug.WriteLine("Finish context: " + task.ContextTypeName);

      return TaskResult.Success;
    }
    #endregion
  }
}