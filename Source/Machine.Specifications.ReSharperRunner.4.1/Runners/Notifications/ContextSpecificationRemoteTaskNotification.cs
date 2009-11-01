using System.Collections.Generic;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  internal class ContextSpecificationRemoteTaskNotification : RemoteTaskNotification
  {
    readonly TaskExecutionNode _node;
    readonly ContextSpecificationTask _task;

    public ContextSpecificationRemoteTaskNotification(TaskExecutionNode node)
    {
      _node = node;
      _task = (ContextSpecificationTask) node.RemoteTask;
    }

    protected override string ContainingType
    {
      get { return _task.ContextTypeName; }
    }

    protected override string Name
    {
      get { return _task.SpecificationFieldName.ToFormat(); }
    }

    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get { yield return _node.RemoteTask; }
    }
  }
}