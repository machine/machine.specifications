using System;
using System.IO;
using System.Reflection;

using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal class RecursiveMSpecTaskRunner : RecursiveRemoteTaskRunner
  {
    Assembly _contextAssembly;
    Type _contextClass;
    PerContextRunListener _listener;
    DefaultRunner _runner;

    public RecursiveMSpecTaskRunner(IRemoteTaskServer server) : base(server)
    {
    }

    #region Overrides of RemoteTaskRunner
    public override TaskResult Start(TaskExecutionNode node)
    {
      ContextTask task = (ContextTask) node.RemoteTask;

      _contextAssembly = LoadContextAssembly(task);
      if (_contextAssembly == null)
      {
        return TaskResult.Error;
      }

      _contextClass = _contextAssembly.GetType(task.ContextTypeName);
      if (_contextClass == null)
      {
        Server.TaskExplain(node.RemoteTask,
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
    #endregion

    #region Overrides of RecursiveRemoteTaskRunner
    public override void ExecuteRecursive(TaskExecutionNode node)
    {
      node.Children.Flatten(x => x.Children).ForEach(TryRegisterSpecifications);
    }

    void TryRegisterSpecifications(TaskExecutionNode node)
    {
      if (node.RemoteTask is ContextSpecificationTask)
      {
        _listener.RegisterSpecification(new ExecutableSpecificationInfo(node));
      }
    }
    #endregion

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