using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Runners.TaskHandlers
{
  internal class SpecificationHandler : ITaskHandler
  {
    RunListener _listener;

    #region ITaskHandler Members
    public bool Accepts(RemoteTask task)
    {
      return task is SpecificationTask;
    }

    public TaskResult Start(IRemoteTaskServer server, TaskExecutionNode node)
    {
      SpecificationTask task = (SpecificationTask) node.RemoteTask;
      Debug.WriteLine("Start spec " + task.SpecificationFieldName);
      Debug.WriteLine("Parent " + ((ContextTask) node.Parent.RemoteTask).ContextTypeName);


      return TaskResult.Success;
      //      
      //
      //      Assembly contextAssembly = LoadContextAssembly(task, server);
      //      if (contextAssembly == null)
      //      {
      //        return TaskResult.Exception;
      //      }
      //
      //      ContextTask contextTask = ((ContextTask)node.Parent.RemoteTask);
      //      
      //      FieldInfo field = contextAssembly.GetType(task.ContextTypeName).GetField(task.SpecificationFieldName,
      //                                                                               BindingFlags.Instance | BindingFlags.DeclaredOnly |
      //                                                                               BindingFlags.NonPublic);
      //      contextTask.Runner.RunMember(contextAssembly, field);
    }

    public TaskResult Execute(IRemoteTaskServer server, TaskExecutionNode node)
    {
      SpecificationTask task = (SpecificationTask) node.RemoteTask;
      Debug.WriteLine("Execute spec " + task.SpecificationFieldName);
      
      Assembly contextAssembly = LoadContextAssembly(task, server);
      if (contextAssembly == null)
      {
        Debug.WriteLine("Error");
        server.TaskError(node.RemoteTask, "foo");
        return TaskResult.Error;
      }

      return TaskResult.Success;
    }

    public TaskResult Finish(IRemoteTaskServer server, TaskExecutionNode node)
    {
      SpecificationTask task = (SpecificationTask) node.RemoteTask;
      Debug.WriteLine("Finish spec " + task.SpecificationFieldName);
      Assembly contextAssembly = LoadContextAssembly(task, server);
      if (contextAssembly == null)
      {
        Debug.WriteLine("Error");
        server.TaskError(node.RemoteTask, "foo");
        return TaskResult.Error;
      }
      return TaskResult.Success;
    }
    #endregion

    static Assembly LoadContextAssembly(SpecificationTask task, IRemoteTaskServer server)
    {
      return null;
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