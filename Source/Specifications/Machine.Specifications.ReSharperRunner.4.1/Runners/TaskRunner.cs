using System.Collections.Generic;
using System.Linq;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners.TaskHandlers;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal class TaskRunner : RemoteTaskRunner
  {
    readonly List<ITaskRunner> _taskHandlers;

    public TaskRunner(IRemoteTaskServer server) : base(server)
    {
      _taskHandlers = new List<ITaskRunner>
                      {
                        new ContextRunner(),
                        new SpecificationRunner()
                      };
    }

    public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
    {
    }

    public override TaskResult Start(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      ITaskRunner runner = FindHandlerFor(task);
      if (runner != null)
      {
        return runner.Start(Server, node);
      }

      return TaskResult.Error;
    }

    public override TaskResult Execute(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      ITaskRunner runner = FindHandlerFor(task);
      if (runner != null)
      {
        return runner.Execute(Server, node);
      }

      return TaskResult.Error;
    }

    public override TaskResult Finish(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      ITaskRunner runner = FindHandlerFor(task);
      if (runner != null)
      {
        return runner.Finish(Server, node);
      }

      return TaskResult.Error;
    }

    ITaskRunner FindHandlerFor(RemoteTask task)
    {
      return _taskHandlers.FirstOrDefault(handler => handler.Accepts(task));
    }
  }
}