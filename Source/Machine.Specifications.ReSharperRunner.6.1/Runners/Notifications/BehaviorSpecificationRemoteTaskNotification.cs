using System.Collections.Generic;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;

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

    protected override string FieldName
    {
      get { return _task.SpecificationFieldName; }
    }

    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get
      {
        yield return _node.RemoteTask;
      }
    }
  }
}