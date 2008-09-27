using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner
{
  public class AppDomainRunner : ISpecificationRunner
  {
    readonly ISpecificationRunListener _listener;
    readonly ISpecificationRunListener _internalListener;

    public AppDomainRunner(ISpecificationRunListener listener)
    {
      _internalListener = listener;
      _listener = new RemoteRunListener(listener);
    }

    public void RunAssembly(Assembly assembly, RunOptions options)
    {
      _internalListener.OnRunStart();

      InternalRunAssembly(assembly, options);

      _internalListener.OnRunEnd();
    }

    public void RunAssemblies(IEnumerable<Assembly> assemblies, RunOptions options)
    {
      _internalListener.OnRunStart();

      assemblies.Each(x => InternalRunAssembly(x, options));

      _internalListener.OnRunEnd();
    }

    public void RunNamespace(Assembly assembly, string targetNamespace, RunOptions options)
    {
      _internalListener.OnRunStart();
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunner("Namespace", appDomain, assembly, options, targetNamespace);

      AppDomain.Unload(appDomain);
      _internalListener.OnRunEnd();
    }

    public void RunMember(Assembly assembly, MemberInfo member, RunOptions options)
    {
      _internalListener.OnRunStart();
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunner("Member", appDomain, assembly, options, member);

      AppDomain.Unload(appDomain);
      _internalListener.OnRunEnd();
    }

    void InternalRunAssembly(Assembly assembly, RunOptions options)
    {
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunner("Assembly", appDomain, assembly, options);

      AppDomain.Unload(appDomain);
    }

    void CreateRunner(string runMethod, AppDomain appDomain, Assembly assembly, RunOptions options, params object[] args)
    {
      string mspecAssemblyFilename = Path.Combine(Path.GetDirectoryName(assembly.Location), "Machine.Specifications.dll");

      var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFilename);

      var constructorArgs = new object[args.Length + 3];
      constructorArgs[0] = _listener;
      constructorArgs[1] = assembly;
      constructorArgs[2] = options;
      Array.Copy(args, 0, constructorArgs, 3, args.Length);

      appDomain.CreateInstanceAndUnwrap(mspecAssemblyName.FullName, "Machine.Specifications.Runner.AppDomainRunner+" + runMethod + "Runner", false, 0, null, constructorArgs, null, null, null);
    }

    static AppDomain CreateAppDomain(Assembly assembly)
    {
      var appDomainSetup = new AppDomainSetup();
      appDomainSetup.ApplicationBase = Path.GetDirectoryName(assembly.Location);
      appDomainSetup.ApplicationName = Guid.NewGuid().ToString();

      appDomainSetup.ConfigurationFile = GetConfigFile(assembly);

      return AppDomain.CreateDomain(appDomainSetup.ApplicationName, null, appDomainSetup);
    }

    static string GetConfigFile(Assembly assembly)
    {
      string configFile = assembly.Location + ".config";

      if (File.Exists(configFile))
        return configFile;

      return null;
    }

    public class AssemblyRunner : MarshalByRefObject
    {
      public AssemblyRunner(ISpecificationRunListener listener, Assembly assembly, RunOptions options)
      {
        var runner = new SpecificationRunner(listener);
        runner.RunAssembly(assembly, options);
      }
    }

    public class NamespaceRunner : MarshalByRefObject
    {
      public NamespaceRunner(ISpecificationRunListener listener, Assembly assembly, RunOptions options, string targetNamespace)
      {
        var runner = new SpecificationRunner(listener);
        runner.RunNamespace(assembly, targetNamespace, options);
      }
    }

    public class MemberRunner : MarshalByRefObject
    {
      public MemberRunner(ISpecificationRunListener listener, Assembly assembly, RunOptions options, MemberInfo memberInfo)
      {
        var runner = new SpecificationRunner(listener);
        runner.RunMember(assembly, memberInfo, options);
      }
    }
  }
}
