using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  class BehaviorSpecificationRemoteTaskNotification : RemoteTaskNotification
  {
    readonly TaskExecutionNode _node;

    readonly BehaviorSpecificationTask _task;

    public BehaviorSpecificationRemoteTaskNotification(TaskExecutionNode node)
    {
      _node = node;
      _task = (BehaviorSpecificationTask) node.RemoteTask;
    }

    string ContextTypeName
    {
      get { return _task.ContextTypeName; }
    }

    string ContainingType
    {
      get { return _task.BehaviorTypeName; }
    }

    string FieldName
    {
      get { return _task.SpecificationFieldName; }
    }

    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get { yield return _node.RemoteTask; }
    }

    public override bool Matches(object infoFromRunner, object maybeContext)
    {
      var context = maybeContext as ContextInfo;
      if (context == null)
      {
        return false;
      }

      var specification = infoFromRunner as SpecificationInfo;
      if (specification == null)
      {
        return false;
      }

      return ContextTypeName == context.TypeName &&
             ContainingType == new NormalizedTypeName(specification.ContainingType) &&
             FieldName == specification.FieldName;
    }

    public override string ToString()
    {
      return String.Format("Behavior specification {0}.{1} with {2} remote tasks",
                           ContainingType,
                           FieldName,
                           RemoteTasks.Count());
    }
  }
}