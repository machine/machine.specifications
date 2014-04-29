namespace Machine.Specifications.ReSharperRunner
{
    using System;
    using System.Reflection;

    using JetBrains.ReSharper.TaskRunnerFramework;

    public interface ISetTempFolderPathStrategy
    {
        void SetTempFolderPath(string path);
        void TestRunFinished();
    }

    public static class SetTempFolderPathStrategyFactory
    {
        public static ISetTempFolderPathStrategy Create(IRemoteTaskServer server)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null && IsReSharper80(entryAssembly.GetName().Version))
                return new BrokenSetTempFolderPathStrategy(server);
            return new SetTempFolderPathStrategy(server);
        }

        private static bool IsReSharper80(Version version)
        {
            // ReSharper 8.0 is broken for SetTempFolderPath. See comments to 
            // BrokenSetTempFolderPathStrategy. 8.0.1 will be 8.0.1000-something
            return version.Major == 8 && version.Minor == 0 && version.Build < 1000;
        }
    }

    public class SetTempFolderPathStrategy : ISetTempFolderPathStrategy
    {
        private readonly IRemoteTaskServer server;

        public SetTempFolderPathStrategy(IRemoteTaskServer server)
        {
            this.server = server;
        }

        public void SetTempFolderPath(string path)
        {
            this.server.SetTempFolderPath(path);
        }

        public void TestRunFinished()
        {
        }
    }

    // ReSharper 8.0 RTM starts deleting the cache folder as soon as you call
    // IRemoteTaskServer.SetTempFolderPath. It has to be called at the end of
    // the run, but that will leak temp folders if the user aborts before the
    // end of the run
    public class BrokenSetTempFolderPathStrategy : ISetTempFolderPathStrategy
    {
        private readonly IRemoteTaskServer server;
        private string tempFolderPath;

        public BrokenSetTempFolderPathStrategy(IRemoteTaskServer server)
        {
            this.server = server;
        }

        public void SetTempFolderPath(string path)
        {
            this.tempFolderPath = path;
        }

        public void TestRunFinished()
        {
            if (!string.IsNullOrEmpty(this.tempFolderPath))
                this.server.SetTempFolderPath(this.tempFolderPath);
        }
    }


}