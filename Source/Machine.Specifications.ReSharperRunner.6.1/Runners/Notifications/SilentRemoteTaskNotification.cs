using System.Collections.Generic;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
  internal class SilentRemoteTaskNotification : RemoteTaskNotification
  {
    public override IEnumerable<RemoteTask> RemoteTasks
    {
      get { yield break; }
    }

    public override bool Matches(object infoFromRunner, ContextInfo maybeContext)
    {
      return false;
    }

    public override string ToString()
    {
      return "Silent";
    }
  }
}