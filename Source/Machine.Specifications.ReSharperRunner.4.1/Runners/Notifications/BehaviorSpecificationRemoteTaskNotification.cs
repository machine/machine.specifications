using System.Collections.Generic;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  internal class BehaviorSpecificationRemoteTaskNotification : RemoteTaskNotification
  {
    readonly TaskExecutionNode _node;
    readonly BehaviorSpecificationTask _task;

    public BehaviorSpecificationRemoteTaskNotification(TaskExecutionNode node)
    {
      _node = node;
      _task = (BehaviorSpecificationTask) node.RemoteTask;
    }

    protected override string ContainingType
    {
      get { return _task.BehaviorTypeName; }
    }

    protected override string Name
    {
      get { return _task.SpecificationFieldName.ToFormat(); }
    }

    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get
      {
        yield return _node.RemoteTask;
        yield return _node.Parent.RemoteTask;
      }
    }
  }
}