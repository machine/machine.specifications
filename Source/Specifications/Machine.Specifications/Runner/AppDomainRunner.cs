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

    public void RunAssembly(Assembly assembly)
    {
      _internalListener.OnRunStart();

      InternalRunAssembly(assembly);

      _internalListener.OnRunEnd();
    }

    public void RunAssemblies(IEnumerable<Assembly> assemblies)
    {
      _internalListener.OnRunStart();

      assemblies.Each(InternalRunAssembly);

      _internalListener.OnRunEnd();
    }

    public void RunNamespace(Assembly assembly, string targetNamespace)
    {
      _internalListener.OnRunStart();
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunner("Namespace", appDomain, assembly, targetNamespace);

      AppDomain.Unload(appDomain);
      _internalListener.OnRunEnd();
    }

    public void RunMember(Assembly assembly, MemberInfo member)
    {
      _internalListener.OnRunStart();
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunner("Member", appDomain, assembly, member);

      AppDomain.Unload(appDomain);
      _internalListener.OnRunEnd();
    }

    void InternalRunAssembly(Assembly assembly)
    {
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunner("Assembly", appDomain, assembly);

      AppDomain.Unload(appDomain);
    }

    void CreateRunner(string runMethod, AppDomain appDomain, Assembly assembly, params object[] args)
    {
      string mspecAssemblyFilename = Path.Combine(Path.GetDirectoryName(assembly.Location), "Machine.Specifications.dll");

      var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFilename);

      var constructorArgs = new object[args.Length + 2];
      constructorArgs[0] = _listener;
      constructorArgs[1] = assembly;
      Array.Copy(args, 0, constructorArgs, 2, args.Length);

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
      public AssemblyRunner(ISpecificationRunListener listener, Assembly assembly)
      {
        var runner = new SpecificationRunner(listener);
        runner.RunAssembly(assembly);
      }
    }

    public class NamespaceRunner : MarshalByRefObject
    {
      public NamespaceRunner(ISpecificationRunListener listener, Assembly assembly, string targetNamespace)
      {
        var runner = new SpecificationRunner(listener);
        runner.RunNamespace(assembly, targetNamespace);
      }
    }

    public class MemberRunner : MarshalByRefObject
    {
      public MemberRunner(ISpecificationRunListener listener, Assembly assembly, MemberInfo memberInfo)
      {
        var runner = new SpecificationRunner(listener);
        runner.RunMember(assembly, memberInfo);
      }
    }
  }
}
