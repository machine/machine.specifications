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

        Assembly _mspecAssembly;

        public RecursiveMSpecTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Remoting is weird (tm). We load the project's Machine.Specifications.dll in
            // the main AppDomain, then tell it do stuff in a new AppDomain that it creates.
            // It reports progress back via a remoting, MarshalByRefObject interface. Except,
            // when the message gets back to the main AppDomain, the de-serialiser tries to
            // resolve the Machine.Specifications.dll assembly again, even though it's loaded
            // into memory. I don't know why. It does this with the full assembly name
            // ("Machine.Specifications, Version=0.4.1.0, ...") and for some reason, that
            // doesn't resolve automatically. Again, I don't know why. Fortunately, we can
            // hook the AssemblyResolve event, and if anyone asks for the mspec dll we have
            // loaded, give it to 'em
            if (_mspecAssembly != null && args.Name == _mspecAssembly.FullName)
                return _mspecAssembly;
            return null;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var task = (RunAssemblyTask) node.RemoteTask;

            var previousCurrentDirectory = Directory.GetCurrentDirectory();
            try
            {
                var directoryName = Path.GetDirectoryName(task.AssemblyLocation);
                if (!string.IsNullOrEmpty(directoryName))
                    Directory.SetCurrentDirectory(directoryName);

                Execute(node, task);
            }
            finally
            {
                Directory.SetCurrentDirectory(previousCurrentDirectory);
            }
        }

        void Execute(TaskExecutionNode node, RunAssemblyTask task)
        {
            var contextAssembly = LoadContextAssembly(task);
            if (contextAssembly == null)
            {
                return;
            }

            _mspecAssembly = LoadMSpecAssembly(Path.GetDirectoryName(task.AssemblyLocation));
            if (_mspecAssembly == null)
            {
                return;
            }

            var factory = new RunnerFactory(Server, _mspecAssembly, node);
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
            // Load context doesn't work. Neither does LoadFrom or LoadFile
            var assemblyFile = Path.Combine(assemblyLocation, "Machine.Specifications.dll");
            var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
            assemblyName.CodeBase = new Uri(assemblyFile).AbsoluteUri;
            return Assembly.Load(assemblyName);
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