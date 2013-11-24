using System;
using System.IO;
using System.Reflection;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharper.Runner;
using Machine.Specifications.ReSharperRunner.Tasks;

// TODO: Fix up namespace
namespace Machine.Specifications.ReSharperRunner.Runners
{
    public class RecursiveMSpecTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Machine.Specifications";

        public RecursiveMSpecTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var task = (RunAssemblyTask) node.RemoteTask;

            var contextAssembly = LoadContextAssembly(task);
            if (contextAssembly == null)
                return;

            var mspecAssembly = LoadMSpecAssembly(Path.GetDirectoryName(task.AssemblyLocation));
            if (mspecAssembly == null)
                return;

            var factory = new RunnerFactory(Server, mspecAssembly, node);
            var runner = factory.CreateRunner();

            try
            {
                runner.StartRun(contextAssembly);
                foreach (var child in node.Children)
                {
                    RunContext(runner, contextAssembly, child);
                }
            }
            finally
            {
                runner.EndRun(contextAssembly);
            }
        }

        Assembly LoadContextAssembly(RunAssemblyTask task)
        {
            if (!File.Exists(task.AssemblyLocation))
            {
                Server.TaskException(task,
                    new[]
                    {
                        new TaskException("no type",
                            String.Format("Could not load assembly from {0}: File does not exist", task.AssemblyLocation),
                            null)
                    });
                return null;
            }

            AssemblyName assemblyName;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(task.AssemblyLocation);
            }
            catch (FileLoadException ex)
            {
                Server.TaskException(task,
                    new[]
                    {
                        new TaskException("no type",
                            String.Format("Could not load assembly from {0}: {1}", task.AssemblyLocation, ex.Message),
                            null)
                    });
                return null;
            }

            if (assemblyName == null)
            {
                Server.TaskException(task,
                    new[]
                    {
                        new TaskException("no type",
                            String.Format("Could not load assembly from {0}: Not an assembly", task.AssemblyLocation),
                            null)
                    });
                return null;
            }

            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception ex)
            {
                Server.TaskException(task,
                    new[]
                    {
                        new TaskException("no type",
                            String.Format("Could not load assembly from {0}: {1}", task.AssemblyLocation, ex.Message),
                            null)
                    });
                return null;
            }
        }

        Assembly LoadMSpecAssembly(string assemblyLocation)
        {
            // TODO: Error handling
            return Assembly.LoadFrom(Path.Combine(assemblyLocation, "Machine.Specifications.dll"));
        }

        void RunContext(IReSharperSpecificationRunner runner, Assembly contextAssembly, TaskExecutionNode node)
        {
            var task = (ContextTask)node.RemoteTask;

            var contextClass = contextAssembly.GetType(task.ContextTypeName);
            if (contextClass == null)
            {
                Server.TaskOutput(task,
                  String.Format("Could not load type '{0}' from assembly {1}.",
                    task.ContextTypeName,
                    task.AssemblyLocation),
                  TaskOutputType.STDOUT);
                Server.TaskException(node.RemoteTask, new[] { new TaskException(new Exception("Could not load context")) });
                return;
            }

            runner.RunMember(contextAssembly, contextClass);
        }
    }
}