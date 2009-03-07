using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal class ExecutableSpecificationInfo
  {
    public ExecutableSpecificationInfo(TaskExecutionNode node)
    {
      RemoteTask = node.RemoteTask;

      if (RemoteTask is ContextSpecificationTask)
      {
        ContextSpecificationTask task = (ContextSpecificationTask)RemoteTask;

        ContainingType = task.ContextTypeName;
        Name = task.SpecificationFieldName.ReplaceUnderscores();
      }

      if (RemoteTask is BehaviorSpecificationTask)
      {
        BehaviorSpecificationTask task = (BehaviorSpecificationTask)RemoteTask;

        ContainingType = task.BehaviorTypeName;
        Name = task.SpecificationFieldName.ReplaceUnderscores();
      }
    }

    public RemoteTask RemoteTask
    {
      get;
      private set;
    }

    public string ContainingType
    {
      get;
      private set;
    }

    public string Name
    {
      get;
      private set;
    }
  }
}