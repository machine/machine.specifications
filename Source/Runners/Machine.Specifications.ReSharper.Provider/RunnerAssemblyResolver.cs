using System;

using JetBrains.Application;
using JetBrains.Application.Env;
using JetBrains.Metadata.Utils;
using JetBrains.Util;

namespace Machine.Specifications.ReSharper.Provider
{
    // Due to issues with remoting and assembly resolution (*) we need to make sure the runner can't
    // load the Machine.Specifications.dll that the provider uses. We'll do that by putting it in a
    // private "runner" directory. However, we need to load it so that we can reference the Tasks.
    // Alternatives are to share the task source code between the provider and the runner so we don't
    // need to load, rename Machine.Specifications.dll to a private name so it doesn't get picked up
    // as part of assembly resolution, or ILMerge it, or push it to a private folder and have a one-of
    // component to simply load it
    // (*) When remoting between the main test runner process AppDomain and the created AppDomain, to
    // report progress, the callback in the main AD attempts to rebind against the Machine.Specifications.dll
    // from the project. I don't know why. It does this by the full AssemblyName ("Machine.Specifications, Version=0.4.1.0, ...")
    // I don't know why this doesn't resolve with the assembly already loaded. ReSharper sets up an
    // assembly resolver that looks for missing assemblies in the runner's location, which means the
    // provider's version of Machine.Specification.dll is picked up instead of the project's, and
    // various casts fail. If the runner is living in its own folder, there's no Machine.Specifications.dll
    // to find, and we can successfully load the right version
    [EnvironmentComponent(Sharing.Common)]
    public class RunnerAssemblyResolver : IDisposable
    {
        readonly AssemblyResolver _resolver;

        public RunnerAssemblyResolver()
        {
            _resolver = new AssemblyResolver(new[] { FileSystemPath.Parse(GetType().Assembly.Location).Directory.Combine("runner") });
            _resolver.Install(AppDomain.CurrentDomain);
        }

        public void Dispose()
        {
            _resolver.Dispose();
        }
    }
}