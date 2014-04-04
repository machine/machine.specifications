namespace Machine.Specifications.ReSharperRunner
{
    using JetBrains.ReSharper.TaskRunnerFramework;

    public class SimpleRemoteTaskServerFactory
    {
        public static ISimpleRemoteTaskServer Create(IRemoteTaskServer server)
        {
            return new SimpleRemoteTaskServer(server);
        }
    }
}