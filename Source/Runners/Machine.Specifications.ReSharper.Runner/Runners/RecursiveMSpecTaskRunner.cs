using System;
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
  class RecursiveMSpecTaskRunner : RecursiveRemoteTaskRunner
  {
    public const string RunnerId = "Machine.Specifications";
    static readonly RemoteTaskNotificationFactory TaskNotificationFactory = new RemoteTaskNotificationFactory();

    public RecursiveMSpecTaskRunner(IRemoteTaskServer server) : base(server)
    {
    }

    public override void ExecuteRecursive(TaskExecutionNode node)
    {
      var task = (RunAssemblyTask) node.RemoteTask;

      var contextAssembly = LoadContextAssembly(task);
      if (contextAssembly == null)
      {
        return;
      }

      var result = VersionCompatibilityChecker.Check(contextAssembly);
      if (!result.Success)
      {
        Server.TaskException(node.RemoteTask, new[] {new TaskException("no type", result.ErrorMessage, null)});

        return;
      }

      var listener = new PerAssemblyRunListener(Server, task);
      var runner = new AppDomainRunner(listener, RunOptions.Default);
      
      node.Flatten(x => x.Children).Each(children => RegisterRemoteTaskNotifications(listener, children));

      try
      {
        runner.StartRun(contextAssembly);
        foreach (var child in node.Children)
        {
          RunContext(runner, contextAssembly, child);
        }
      }
      finally
      {
        runner.EndRun(contextAssembly);
      }
    }

    void RunContext(ISpecificationRunner runner, Assembly contextAssembly, TaskExecutionNode node)
    {
      var task = (ContextTask) node.RemoteTask;

      var contextClass = contextAssembly.GetType(task.ContextTypeName);
      if (contextClass == null)
      {
        Server.TaskOutput(task,
          String.Format("Could not load type '{0}' from assembly {1}.",
            task.ContextTypeName,
            task.AssemblyLocation),
          TaskOutputType.STDOUT);
        Server.TaskException(node.RemoteTask, new[] {new TaskException(new Exception("Could not load context"))});
        return;
      }

      runner.RunMember(contextAssembly, contextClass);
    }

    void RegisterRemoteTaskNotifications(PerAssemblyRunListener listener, TaskExecutionNode node)
    {
      listener.RegisterTaskNotification(TaskNotificationFactory.CreateTaskNotification(node));
    }

    Assembly LoadContextAssembly(RunAssemblyTask task)
    {
      if (!File.Exists(task.AssemblyLocation))
      {
        Server.TaskException(task,
          new[]
          {
            new TaskException("no type",
              String.Format("Could not load assembly from {0}: File does not exist", task.AssemblyLocation),
              null)
          });
        return null;
      }

      AssemblyName assemblyName;
      try
      {
        assemblyName = AssemblyName.GetAssemblyName(task.AssemblyLocation);
      }
      catch (FileLoadException ex)
      {
        Server.TaskException(task,
          new[]
          {
            new TaskException("no type",
              String.Format("Could not load assembly from {0}: {1}", task.AssemblyLocation, ex.Message),
              null)
          });
        return null;
      }

      if (assemblyName == null)
      {
        Server.TaskException(task,
          new[]
          {
            new TaskException("no type",
              String.Format("Could not load assembly from {0}: Not an assembly", task.AssemblyLocation),
              null)
          });
        return null;
      }

      try
      {
        return Assembly.Load(assemblyName);
      }
      catch (Exception ex)
      {
        Server.TaskException(task,
          new[]
          {
            new TaskException("no type",
              String.Format("Could not load assembly from {0}: {1}", task.AssemblyLocation, ex.Message),
              null)
          });
        return null;
      }
    }
  }
}