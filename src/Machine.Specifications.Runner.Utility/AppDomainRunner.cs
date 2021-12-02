using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NETSTANDARD
using System.Runtime.Remoting.Messaging;
#endif
using System.Security;
using System.Security.Permissions;

namespace Machine.Specifications.Runner.Utility
{
    public class AppDomainRunner : ISpecificationRunner
    {
        private const string RunnerType = "Machine.Specifications.Runner.Impl.DefaultRunner";

        private readonly ISpecificationRunListener listener;

        private readonly RunOptions options;

        private readonly InvokeOnce signalRunStart;

        private readonly InvokeOnce signalRunEnd;

        private readonly Dictionary<AssemblyPath, AppDomainAndRunner> appDomains = new Dictionary<AssemblyPath, AppDomainAndRunner>();

        public AppDomainRunner(ISpecificationRunListener listener, RunOptions options)
        {
            this.listener = new RemoteRunListener(listener);
            this.options = options;

            signalRunStart = new InvokeOnce(listener.OnRunStart);
            signalRunEnd = new InvokeOnce(listener.OnRunEnd);
        }

        [SecuritySafeCritical]
        public void RunAssembly(AssemblyPath assembly)
        {
            RunAssemblies(new[] { assembly });
        }

        [SecuritySafeCritical]
        public void RunAssemblies(IEnumerable<AssemblyPath> assemblies)
        {
            signalRunStart.Invoke();

            assemblies.Each(assembly =>
            {
                try
                {
                    StartRun(assembly);
                    GetOrCreateAppDomainRunner(assembly).Runner.RunAssembly(assembly);
                }
                finally
                {
                    EndRun(assembly);
                }
            });

            signalRunEnd.Invoke();
        }

        [SecuritySafeCritical]
        public void RunNamespace(AssemblyPath assembly, string targetNamespace)
        {
            signalRunStart.Invoke();

            StartRun(assembly);
            GetOrCreateAppDomainRunner(assembly).Runner.RunNamespace(assembly, targetNamespace);
        }

        [SecuritySafeCritical]
        public void RunMember(AssemblyPath assembly, MemberInfo member)
        {
            signalRunStart.Invoke();

            StartRun(assembly);
            GetOrCreateAppDomainRunner(assembly).Runner.RunMember(assembly, member);
        }

        [SecuritySafeCritical]
        public void StartRun(AssemblyPath assembly)
        {
            if (RunnerWasCreated(assembly))
            {
                return;
            }

            GetOrCreateAppDomainRunner(assembly).Runner.StartRun(assembly);
        }

        [SecuritySafeCritical]
        public void EndRun(AssemblyPath assembly)
        {
            if (!RunnerWasCreated(assembly))
            {
                return;
            }

            var appDomainRunner = GetOrCreateAppDomainRunner(assembly);
            RemoveEntryFor(assembly);

            try
            {
                appDomainRunner.Runner.EndRun(assembly);
            }
            finally
            {
                UnloadAppDomain(appDomainRunner.AppDomain);
            }
        }

        private void RemoveEntryFor(AssemblyPath assembly)
        {
            appDomains.Remove(assembly);
        }

        private static void UnloadAppDomain(AppDomain appDomain)
        {
            if (appDomain == null)
            {
                return;
            }

#if !NETSTANDARD
            var cachePath = appDomain.SetupInformation.CachePath;

            try
            {
                AppDomain.Unload(appDomain);

                if (cachePath != null)
                {
                    Directory.Delete(cachePath, true);
                }
            }
            catch
            {
                // This is OK for cleanup
            }
#endif
        }

        [SecuritySafeCritical]
        private ISpecificationRunner CreateRunnerInSeparateAppDomain(AppDomain appDomain, AssemblyPath assembly)
        {
#if !NETSTANDARD
            var path = Path.GetDirectoryName(assembly);

            if (path == null)
            {
                return new NullSpecificationRunner();
            }

            var mspecAssemblyFilename = Path.Combine(path, "Machine.Specifications.dll");

            if (!File.Exists(mspecAssemblyFilename))
            {
                return new NullSpecificationRunner();
            }

            var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFilename);

            var constructorArgs = new object[3];
            constructorArgs[0] = listener;
            constructorArgs[1] = options.ToXml();
            constructorArgs[2] = false;

            try
            {
                var defaultRunner = (IMessageSink)appDomain.CreateInstanceAndUnwrap(
                    mspecAssemblyName.FullName,
                    RunnerType,
                    false,
                    0,
                    null,
                    constructorArgs,
                    null,
                    null,
                    null);

                return new RemoteRunnerDecorator(defaultRunner);
            }
            catch (Exception err)
            {
                Console.Error.WriteLine("Runner failure: " + err);
                throw;
            }
#else
            var runnerType = Type.GetType($"{RunnerType}, Machine.Specifications");

            if (runnerType == null)
            {
                throw new InvalidOperationException("Machine.Specifications library not found");
            }

            var ctor = runnerType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.GetParameters().Length == 3);

            var runner = (Runner.ISpecificationRunner) ctor.Invoke(new object[]
            {
                new ReflectionRunListener(listener),
                new Runner.RunOptions(options.IncludeTags, options.ExcludeTags, options.Filters),
                false
            });

            return new ReflectionSpecificationRunner(runner);
#endif
        }

        private AppDomainAndRunner GetOrCreateAppDomainRunner(AssemblyPath assembly)
        {
            if (appDomains.TryGetValue(assembly, out var appDomainAndRunner))
            {
                return appDomainAndRunner;
            }

#if !NETSTANDARD
            var setup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(assembly),
                ApplicationName = "Machine Specifications Runner",
                ConfigurationFile = GetConfigFile(assembly)
            };

            if (!string.IsNullOrEmpty(options.ShadowCopyCachePath))
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = setup.ApplicationBase;
                setup.CachePath = options.ShadowCopyCachePath;
            }

            var appDomain = AppDomain.CreateDomain(setup.ApplicationName, null, setup, new PermissionSet(PermissionState.Unrestricted));

            var runner = CreateRunnerInSeparateAppDomain(appDomain, assembly);

            appDomains.Add(assembly, new AppDomainAndRunner
            {
                AppDomain = appDomain,
                Runner = runner
            });

            return GetOrCreateAppDomainRunner(assembly);
#else
            return new AppDomainAndRunner
            {
                Runner = CreateRunnerInSeparateAppDomain(null, assembly)
            };
#endif
        }

        private bool RunnerWasCreated(AssemblyPath assembly)
        {
            return appDomains.ContainsKey(assembly);
        }

        private static string GetConfigFile(AssemblyPath assembly)
        {
            var configFile = assembly + ".config";

            if (File.Exists(configFile))
            {
                return configFile;
            }

            return null;
        }

        private class AppDomainAndRunner
        {
            public AppDomain AppDomain { get; set; }

            public ISpecificationRunner Runner { get; set; }
        }
    }
}
