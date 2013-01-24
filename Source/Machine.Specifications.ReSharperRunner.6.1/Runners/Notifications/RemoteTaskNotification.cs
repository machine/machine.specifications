using System.Collections.Generic;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  internal abstract class RemoteTaskNotification
  {
    protected abstract string ContainingType
    {
      get;
    }

    protected abstract string FieldName
    {
      get;
    }

    public abstract IEnumerable<RemoteTask> RemoteTasks
    {
      get;
    }

    public virtual bool Matches(SpecificationInfo specification)
    {
      return ContainingType == new NormalizedTypeName(specification.ContainingType) &&
             FieldName == specification.FieldName;
    }
  }
}