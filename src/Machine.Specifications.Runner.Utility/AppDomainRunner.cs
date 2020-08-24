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

        readonly ISpecificationRunListener _listener;
        readonly RunOptions _options;
        readonly InvokeOnce _signalRunStart;
        readonly InvokeOnce _signalRunEnd;

        public AppDomainRunner(ISpecificationRunListener listener, RunOptions options)
        {
            _listener = new RemoteRunListener(listener);
            _options = options;

            _signalRunStart = new InvokeOnce(listener.OnRunStart);
            _signalRunEnd = new InvokeOnce(listener.OnRunEnd);
        }

        [SecuritySafeCritical]
        public void RunAssembly(AssemblyPath assembly)
        {
            RunAssemblies(new[] { assembly });
        }

        [SecuritySafeCritical]
        public void RunAssemblies(IEnumerable<AssemblyPath> assemblies)
        {
            _signalRunStart.Invoke();

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

            _signalRunEnd.Invoke();
        }

        [SecuritySafeCritical]
        public void RunNamespace(AssemblyPath assembly, string targetNamespace)
        {
            _signalRunStart.Invoke();
            StartRun(assembly);
            GetOrCreateAppDomainRunner(assembly).Runner.RunNamespace(assembly, targetNamespace);
        }

        [SecuritySafeCritical]
        public void RunMember(AssemblyPath assembly, MemberInfo member)
        {
            _signalRunStart.Invoke();
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

        void RemoveEntryFor(AssemblyPath assembly)
        {
            _appDomains.Remove(assembly);
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
        ISpecificationRunner CreateRunnerInSeparateAppDomain(AppDomain appDomain, AssemblyPath assembly)
        {
            var mspecAssemblyFilename = Path.Combine(Path.GetDirectoryName(assembly), "Machine.Specifications.dll");
            if (!File.Exists(mspecAssemblyFilename))
            {
                return new NullSpecificationRunner();
            }

            var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFilename);

#if !NETSTANDARD
            var constructorArgs = new object[3];
            constructorArgs[0] = _listener;
            constructorArgs[1] = _options.ToXml();
            constructorArgs[2] = false;

            try
            {
                var defaultRunner = (IMessageSink)appDomain.CreateInstanceAndUnwrap(mspecAssemblyName.FullName,
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
            var runnerType = Type.GetType($"{RunnerType}, {mspecAssemblyName.FullName}");

            var ctor = runnerType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.GetParameters().Length == 3);

            Runner.RunOptions options = new Runner.RunOptions(_options.IncludeTags, _options.ExcludeTags, _options.Filters);

            var listener = new ReflectionRunListener(_listener);
            var runner = (Runner.ISpecificationRunner) ctor.Invoke(new object[] {listener, options, false});

            return new ReflectionSpecificationRunner(runner);
#endif
        }

        readonly Dictionary<AssemblyPath, AppDomainAndRunner> _appDomains = new Dictionary<AssemblyPath, AppDomainAndRunner>();

        AppDomainAndRunner GetOrCreateAppDomainRunner(AssemblyPath assembly)
        {
            AppDomainAndRunner appDomainAndRunner;
            if (_appDomains.TryGetValue(assembly, out appDomainAndRunner))
            {
                return appDomainAndRunner;
            }

#if !NETSTANDARD
            var setup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(assembly),
                ApplicationName = string.Format("Machine Specifications Runner"),
                ConfigurationFile = GetConfigFile(assembly)
            };

            if (!string.IsNullOrEmpty(_options.ShadowCopyCachePath))
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = setup.ApplicationBase;
                setup.CachePath = _options.ShadowCopyCachePath;
            }

            var appDomain = AppDomain.CreateDomain(setup.ApplicationName, null, setup, new PermissionSet(PermissionState.Unrestricted));

            var runner = CreateRunnerInSeparateAppDomain(appDomain, assembly);

            _appDomains.Add(assembly, new AppDomainAndRunner
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

        bool RunnerWasCreated(AssemblyPath assembly)
        {
            return _appDomains.ContainsKey(assembly);
        }

        static string GetConfigFile(AssemblyPath assembly)
        {
            var configFile = assembly + ".config";

            if (File.Exists(configFile))
            {
                return configFile;
            }

            return null;
        }

        class AppDomainAndRunner
        {
            public AppDomain AppDomain { get; set; }
            public ISpecificationRunner Runner { get; set; }
        }
    }
}
