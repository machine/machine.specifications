using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  internal partial class Task
  {
    bool BaseEquals(RemoteTask other)
    {
      return Equals(other);
    }
  }
}