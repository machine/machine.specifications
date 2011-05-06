using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners.Notifications;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal class RecursiveMSpecTaskRunner : RecursiveRemoteTaskRunner
  {
    readonly RemoteTaskNotificationFactory _taskNotificationFactory = new RemoteTaskNotificationFactory();
    Assembly _contextAssembly;
    Type _contextClass;
    PerContextRunListener _listener;
    DefaultRunner _runner;

    public RecursiveMSpecTaskRunner(IRemoteTaskServer server) : base(server)
    {
    }

    public override TaskResult Start(TaskExecutionNode node)
    {
      var task = (ContextTask) node.RemoteTask;

      _contextAssembly = LoadContextAssembly(task);
      if (_contextAssembly == null)
      {
        return TaskResult.Error;
      }

      var result = VersionCompatibilityChecker.Check(_contextAssembly);
      if (!result.Success)
      {
        Server.TaskExplain(task, result.Explanation);
        Server.TaskError(task, result.ErrorMessage);

        return TaskResult.Error;
      }

      _contextClass = _contextAssembly.GetType(task.ContextTypeName);
      if (_contextClass == null)
      {
        Server.TaskExplain(task,
                           String.Format("Could not load type '{0}' from assembly {1}.",
                                         task.ContextTypeName,
                                         task.AssemblyLocation));
        Server.TaskError(node.RemoteTask, "Could not load context");
        return TaskResult.Error;
      }

      _listener = new PerContextRunListener(Server, node.RemoteTask);
      _runner = new DefaultRunner(_listener, RunOptions.Default);

      return TaskResult.Success;
    }

    public override TaskResult Execute(TaskExecutionNode node)
    {
      // This method is never called.
      return TaskResult.Success;
    }

    public override TaskResult Finish(TaskExecutionNode node)
    {
      _runner.RunMember(_contextAssembly, _contextClass);

      return TaskResult.Success;
    }

    public override void ExecuteRecursive(TaskExecutionNode node)
    {
      FlattenChildren(node).Each(RegisterRemoteTaskNotifications);
    }

    static IEnumerable<TaskExecutionNode> FlattenChildren(TaskExecutionNode node)
    {
      foreach (var child in node.Children)
      {
        yield return child;

        foreach (var descendant in child.Children)
        {
          yield return descendant;
        }
      }
    }

    void RegisterRemoteTaskNotifications(TaskExecutionNode node)
    {
      _listener.RegisterTaskNotification(_taskNotificationFactory.CreateTaskNotification(node));
    }

    Assembly LoadContextAssembly(Task task)
    {
      AssemblyName assemblyName;
      if (!File.Exists(task.AssemblyLocation))
      {
        Server.TaskExplain(task,
                           String.Format("Could not load assembly from {0}: File does not exist", task.AssemblyLocation));
        Server.TaskError(task, "Could not load context assembly");
        return null;
      }

      try
      {
        assemblyName = AssemblyName.GetAssemblyName(task.AssemblyLocation);
      }
      catch (FileLoadException ex)
      {
        Server.TaskExplain(task,
                           String.Format("Could not load assembly from {0}: {1}", task.AssemblyLocation, ex.Message));
        Server.TaskError(task, "Could not load context assembly");
        return null;
      }

      if (assemblyName == null)
      {
        Server.TaskExplain(task,
                           String.Format("Could not load assembly from {0}: Not an assembly", task.AssemblyLocation));
        Server.TaskError(task, "Could not load context assembly");
        return null;
      }

      try
      {
        return Assembly.Load(assemblyName);
      }
      catch (Exception ex)
      {
        Server.TaskExplain(task,
                           String.Format("Could not load assembly from {0}: {1}", task.AssemblyLocation, ex.Message));
        Server.TaskError(task, "Could not load context assembly");
        return null;
      }
    }
  }
}