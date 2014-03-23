using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  class AssemblyRemoteTaskNotification : RemoteTaskNotification
  {
    readonly TaskExecutionNode _node;
    readonly RunAssemblyTask _task;

    public AssemblyRemoteTaskNotification(TaskExecutionNode node)
    {
      _node = node;
      _task = (RunAssemblyTask) node.RemoteTask;
    }

    string Location
    {
      get { return _task.AssemblyLocation; }
    }

    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get { yield return _node.RemoteTask; }
    }

    public override bool Matches(object infoFromRunner, object maybeContext)
    {
      var assembly = infoFromRunner as AssemblyInfo;
      if (assembly == null)
      {
        return false;
      }

      return Location == assembly.Location;
    }

    public override string ToString()
    {
      return String.Format("Assembly {0} with {1} remote tasks",
                           Location,
                           RemoteTasks.Count());
    }
  }
}