using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.ReSharperRunner.Runners.TaskHandlers
{
  internal class SpecificationRunner : ITaskRunner
  {
    Assembly _contextAssembly;
    ReSharperRunListener _listener;
    FieldInfo _field;
    DefaultRunner _runner;

    #region ITaskRunner Members
    public bool Accepts(RemoteTask task)
    {
      return task is SpecificationTask;
    }

    public TaskResult Start(IRemoteTaskServer server, TaskExecutionNode node)
    {
      SpecificationTask task = (SpecificationTask) node.RemoteTask;
      Debug.WriteLine("Start spec " + task.SpecificationFieldName);
      Debug.WriteLine("Parent " + ((ContextTask) node.Parent.RemoteTask).ContextTypeName);

      _contextAssembly = LoadContextAssembly(task, server);
      if (_contextAssembly == null)
      {
        return TaskResult.Error;
      }
      
      _field = _contextAssembly.GetType(task.ContextTypeName).GetField(task.SpecificationFieldName,
                                                                     BindingFlags.Instance |
                                                                     BindingFlags.DeclaredOnly |
                                                                     BindingFlags.NonPublic);

      _listener = new ReSharperRunListener(server, node);
      _runner = new DefaultRunner(_listener, RunOptions.Default);

      return TaskResult.Success;
    }

    public TaskResult Execute(IRemoteTaskServer server, TaskExecutionNode node)
    {
      SpecificationTask task = (SpecificationTask) node.RemoteTask;
      Debug.WriteLine("Execute spec " + task.SpecificationFieldName);

      _runner.RunMember(_contextAssembly, _field);

      return TaskResult.Success;
    }

    public TaskResult Finish(IRemoteTaskServer server, TaskExecutionNode node)
    {
      SpecificationTask task = (SpecificationTask) node.RemoteTask;
      Debug.WriteLine("Finish spec " + task.SpecificationFieldName);
      
      _listener.RunFinished.WaitOne();

      return _listener.TaskResult;
    }
    #endregion

    static Assembly LoadContextAssembly(SpecificationTask task, IRemoteTaskServer server)
    {
      AssemblyName assemblyName;
      if (!File.Exists(task.AssemblyLocation))
      {
        server.TaskError(task, string.Format("Cannot load assembly from {0}: File not exists", task.AssemblyLocation));
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
        server.TaskError(task, string.Format("Cannot load assembly from {0}: not an assembly", task.AssemblyLocation));
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