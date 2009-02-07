using System;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal static class ContextRunner
  {
    public static TaskResult Execute(IRemoteTaskServer server, TaskExecutionNode node, ContextTask task)
    {
      throw new NotImplementedException();
    }

    public static TaskResult Finish(IRemoteTaskServer server, TaskExecutionNode node, ContextTask task)
    {
      throw new NotImplementedException();
    }

    public static TaskResult Start(IRemoteTaskServer server, TaskExecutionNode node, ContextTask task)
    {
      throw new NotImplementedException();
    }
  }
}