using System;
using System.Diagnostics;
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
      Debug.WriteLine("Start: " + node.RemoteTask.GetType().FullName);

      ContextTask task = (ContextTask) node.RemoteTask;

      _contextAssembly = LoadContextAssembly(task, Server);
      if (_contextAssembly == null)
      {
        return TaskResult.Error;
      }

      _listener = new PerContextRunListener(Server, node.RemoteTask);
      _runner = new DefaultRunner(_listener, RunOptions.Default);
      _contextClass = _contextAssembly.GetType(task.ContextTypeName);

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
      node.Flatten(x => x.Children).ForEach(TryRegisterSpecifications);
    }

    void TryRegisterSpecifications(TaskExecutionNode node)
    {
      if (node.RemoteTask is ContextSpecificationTask)
      {
        _listener.RegisterSpecification(new ExecutableSpecificationInfo(node));
      }
    }
    #endregion

    static Assembly LoadContextAssembly(Task task, IRemoteTaskServer server)
    {
      AssemblyName assemblyName;
      if (!File.Exists(task.AssemblyLocation))
      {
        server.TaskError(task, string.Format("Cannot load assembly from {0}: File does not exist", task.AssemblyLocation));
        return null;
      }

      try
      {
        assemblyName = AssemblyName.GetAssemblyName(task.AssemblyLocation);
      }
      catch (FileLoadException ex)
      {
        server.TaskError(task, string.Format("Cannot load assembly from {0}: {1}", task.AssemblyLocation, ex.Message));
        return null;
      }

      if (assemblyName == null)
      {
        server.TaskError(task, string.Format("Cannot load assembly from {0}: Not an assembly", task.AssemblyLocation));
        return null;
      }

      try
      {
        return Assembly.Load(assemblyName);
      }
      catch (Exception ex)
      {
        server.TaskError(task, string.Format("Cannot load assembly from {0}: {1}", task.AssemblyLocation, ex.Message));
        return null;
      }
    }
  }
}