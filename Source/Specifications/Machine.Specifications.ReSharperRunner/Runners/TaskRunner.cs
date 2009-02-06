using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal class TaskRunner : RemoteTaskRunner
  {
    public TaskRunner(IRemoteTaskServer server) : base(server)
    {
    }

    public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
    {
    }

    public override TaskResult Execute(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      if (task is ContextTask)
      {
        return ContextRunner.Execute(Server, node, (ContextTask) task);
      }

      if (task is SpecificationTask)
      {
        return SpecificationRunner.Execute(Server, node, (SpecificationTask) task);
      }

      return TaskResult.Error;
    }

    public override TaskResult Finish(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      if (task is ContextTask)
      {
        return ContextRunner.Finish(Server, node, (ContextTask) task);
      }

      if (task is SpecificationTask)
      {
        return SpecificationRunner.Finish(Server, node, (SpecificationTask) task);
      }

      return TaskResult.Error;
    }

    public override TaskResult Start(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      if (task is ContextTask)
      {
        return ContextRunner.Start(Server, node, (ContextTask) task);
      }

      if (task is SpecificationTask)
      {
        return SpecificationRunner.Start(Server, node, (SpecificationTask) task);
      }

      return TaskResult.Error;
    }
  }
}