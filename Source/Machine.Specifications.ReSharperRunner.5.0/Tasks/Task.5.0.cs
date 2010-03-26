using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  internal partial class Task
  {
    public override bool Equals(RemoteTask other)
    {
      return Equals(other as Task);
    }
    
    bool BaseEquals(RemoteTask other)
    {
      return !ReferenceEquals(null, other) && Equals(other.RunnerID, RunnerID);
    }
  }
}