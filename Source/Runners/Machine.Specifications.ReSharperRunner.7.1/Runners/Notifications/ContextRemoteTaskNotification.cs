using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  class ContextRemoteTaskNotification : RemoteTaskNotification
  {
    readonly TaskExecutionNode _node;
    readonly ContextTask _task;

    public ContextRemoteTaskNotification(TaskExecutionNode node)
    {
      _node = node;
      _task = (ContextTask) node.RemoteTask;
    }

    string ContainingType
    {
      get { return _task.ContextTypeName; }
    }

    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get { yield return _node.RemoteTask; }
    }

    public override bool Matches(object infoFromRunner, object maybeContext)
    {
      var context = infoFromRunner as ContextInfo;

      if (context == null)
      {
        return false;
      }

      return ContainingType == context.TypeName;
    }

    public override string ToString()
    {
      return String.Format("Context {0} with {1} remote tasks",
                           ContainingType,
                           RemoteTasks.Count());
    }
  }
}