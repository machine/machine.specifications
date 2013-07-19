using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl
{
  public class AppDomainRunner : ISpecificationRunner
  {
    readonly ISpecificationRunListener _listener;
    readonly RunOptions _options;

    public AppDomainRunner(ISpecificationRunListener listener, RunOptions options)
    {
      _listener = new RemoteRunListener(listener);
      _options = options;
    }

    [SecuritySafeCritical]
    public void RunAssembly(Assembly assembly)
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
    }

    [SecuritySafeCritical]
    public void RunAssemblies(IEnumerable<Assembly> assemblies)
    {
      assemblies.Each(RunAssembly);
    }

    [SecuritySafeCritical]
    public void RunNamespace(Assembly assembly, string targetNamespace)
    {
      StartRun(assembly);
      GetOrCreateAppDomainRunner(assembly).Runner.RunNamespace(assembly, targetNamespace);
    }

    [SecuritySafeCritical]
    public void RunMember(Assembly assembly, MemberInfo member)
    {
      StartRun(assembly);
      GetOrCreateAppDomainRunner(assembly).Runner.RunMember(assembly, member);
    }

    [SecuritySafeCritical]
    public void StartRun(Assembly assembly)
    {
      if (RunnerWasCreated(assembly))
      {
        return;
      }

      GetOrCreateAppDomainRunner(assembly).Runner.StartRun(assembly);
    }

    [SecuritySafeCritical]
    public void EndRun(Assembly assembly)
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
        AppDomain.Unload(appDomainRunner.AppDomain);
        RemoveEntryFor(assembly);
      }
    }

    void RemoveEntryFor(Assembly assembly)
    {
      _appDomains.Remove(assembly);
    }

    [SecuritySafeCritical]
    DefaultRunner CreateRunnerInSeparateAppDomain(AppDomain appDomain, Assembly assembly)
    {
      var mspecAssemblyFilename = Path.Combine(Path.GetDirectoryName(assembly.Location), "Machine.Specifications.dll");
      var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFilename);

      var constructorArgs = new object[2];
      constructorArgs[0] = _listener;
      constructorArgs[1] = _options;

      using (new SpecAssemblyResolver(assembly))
      {
        try
        {
          return (DefaultRunner) appDomain.CreateInstanceAndUnwrap(mspecAssemblyName.FullName,
                                                                   "Machine.Specifications.Runner.Impl.DefaultRunner",
                                                                   false,
                                                                   0,
                                                                   null,
                                                                   constructorArgs,
                                                                   null,
                                                                   null,
                                                                   null);
        }
        catch (Exception err)
        {
          Console.Error.WriteLine("Runner failure: " + err);
          throw;
        }
      }
    }

    readonly Dictionary<Assembly, AppDomainAndRunner> _appDomains = new Dictionary<Assembly, AppDomainAndRunner>();

    AppDomainAndRunner GetOrCreateAppDomainRunner(Assembly assembly)
    {
      AppDomainAndRunner appDomainAndRunner;
      if (_appDomains.TryGetValue(assembly, out appDomainAndRunner))
      {
        return appDomainAndRunner;
      }

      var appDomainSetup = new AppDomainSetup
                           {
                             ApplicationBase = Path.GetDirectoryName(assembly.Location),
                             ApplicationName = Guid.NewGuid().ToString(),
                             ConfigurationFile = GetConfigFile(assembly)
                           };

      var appDomain = AppDomain.CreateDomain(appDomainSetup.ApplicationName, null, appDomainSetup);
      var runner = CreateRunnerInSeparateAppDomain(appDomain, assembly);

      _appDomains.Add(assembly, new AppDomainAndRunner
                                {
                                  AppDomain = appDomain,
                                  Runner = runner
                                });

      return GetOrCreateAppDomainRunner(assembly);
    }

    bool RunnerWasCreated(Assembly assembly)
    {
      return _appDomains.ContainsKey(assembly);
    }

    static string GetConfigFile(Assembly assembly)
    {
      var configFile = assembly.Location + ".config";

      if (File.Exists(configFile))
      {
        return configFile;
      }

      return null;
    }

    class AppDomainAndRunner
    {
      public AppDomain AppDomain { get; set; }
      public DefaultRunner Runner { get; set; }
    }
  }
}