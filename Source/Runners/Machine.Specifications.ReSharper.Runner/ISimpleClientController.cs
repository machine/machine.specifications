namespace Machine.Specifications.ReSharperRunner
{
    using JetBrains.ReSharper.TaskRunnerFramework;

    public interface ISimpleClientController
    {
        void TaskRunStarting();
        void TaskStarting(RemoteTask remoteTask);
        void TaskFinished(RemoteTask remoteTask);
        void TaskRunFinished();
    }

    public class NullSimpleClientController : ISimpleClientController
    {
        public void TaskRunStarting()
        {
        }

        public void TaskStarting(RemoteTask remoteTask)
        {
        }

        public void TaskFinished(RemoteTask remoteTask)
        {
        }

        public void TaskRunFinished()
        {
        }
    }
}