using System.Collections.Generic;
using System.Linq;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners.TaskHandlers;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal class TaskRunner : RemoteTaskRunner
  {
    readonly List<ITaskHandler> _taskHandlers;

    public TaskRunner(IRemoteTaskServer server) : base(server)
    {
      _taskHandlers = new List<ITaskHandler>
                      {
                        new ContextHandler(),
                        new SpecificationHandler()
                      };
    }

    public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
    {
    }

    public override TaskResult Start(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      ITaskHandler handler = FindHandlerFor(task);
      if (handler != null)
      {
        return handler.Start(Server, node);
      }

      return TaskResult.Error;
    }

    public override TaskResult Execute(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      ITaskHandler handler = FindHandlerFor(task);
      if (handler != null)
      {
        return handler.Execute(Server, node);
      }

      return TaskResult.Error;
    }

    public override TaskResult Finish(TaskExecutionNode node)
    {
      RemoteTask task = node.RemoteTask;

      ITaskHandler handler = FindHandlerFor(task);
      if (handler != null)
      {
        return handler.Finish(Server, node);
      }

      return TaskResult.Error;
    }

    ITaskHandler FindHandlerFor(RemoteTask task)
    {
      return _taskHandlers.FirstOrDefault(handler => handler.Accepts(task));
    }
  }
}