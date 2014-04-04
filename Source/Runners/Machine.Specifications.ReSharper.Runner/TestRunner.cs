namespace Machine.Specifications.ReSharperRunner
{
    using System;
    using System.IO;
    using System.Linq;

    using JetBrains.ReSharper.TaskRunnerFramework;

    using Machine.Specifications.ReSharperRunner.Tasks;
    using Machine.Specifications.Runner.Utility;

    public class TestRunner
    {
        private readonly RemoteTaskServer _server;
        private readonly TaskExecutorConfiguration _configuration;

        public TestRunner(RemoteTaskServer server)
        {
            this._server = server;
            this._configuration = server.Configuration;
        }

        public void Run(RunAssemblyTask assemblyTask, TaskProvider taskProvider)
        {
            var priorCurrentDirectory = Environment.CurrentDirectory;
            try
            {
                // Use the assembly in the folder that the user has specified, or, if not, use the assembly location
                var assemblyFolder = GetAssemblyFolder(this._configuration, assemblyTask);
                var assemblyPath = Path.Combine(assemblyFolder, GetFileName(assemblyTask.AssemblyLocation));

                Environment.CurrentDirectory = assemblyFolder;

                using (IVersionResilentSpecRunner versionResilentSpecRunner = new VersionResilentSpecRunner())
                {
                    var listener = new PerAssemblyRunListener(this._server, taskProvider);
                    var contextList = taskProvider.ContextNames.ToList();
                    versionResilentSpecRunner.RunSpecs(assemblyPath, listener, contextList);
                }
            }
            finally
            {
                Environment.CurrentDirectory = priorCurrentDirectory;
            }
        }

        private static string GetAssemblyFolder(TaskExecutorConfiguration config, RunAssemblyTask assemblyTask)
        {
            return string.IsNullOrEmpty(config.AssemblyFolder)
                       ? GetDirectoryName(assemblyTask.AssemblyLocation)
                       : config.AssemblyFolder;
        }

        private static string GetDirectoryName(string filepath)
        {
            return Path.GetDirectoryName(filepath);
        }

        private static string GetFileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }
    }
}