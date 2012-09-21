using System.Collections.Generic;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  internal class SilentRemoteTaskNotification : RemoteTaskNotification
  {
    protected override string ContainingType
    {
      get { return null; }
    }

    protected override string Name
    {
      get { return null; }
    }

    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get { yield break; }
    }
  }
}