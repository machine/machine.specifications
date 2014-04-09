namespace Machine.Specifications.ReSharperRunner
{
    using JetBrains.ReSharper.TaskRunnerFramework;

    public class SimpleClientControllerFactory
    {
        public static ISimpleClientController Create(IRemoteTaskServer server)
        {
            return new NullSimpleClientController();
        }
    }
}