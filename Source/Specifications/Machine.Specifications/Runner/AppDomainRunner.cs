using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner
{
  public class AppDomainRunner : ISpecificationRunner
  {
    readonly ISpecificationRunListener listener;

    public AppDomainRunner(ISpecificationRunListener listener)
    {
      this.listener = new RemoteRunListener(listener);
    }

    public void RunAssembly(Assembly assembly)
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
      constructorArgs[0] = listener;
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

    public void RunNamespace(Assembly assembly, string targetNamespace)
    {
      AppDomain appDomain = CreateAppDomain(assembly);
      CreateRunner("Namespace", appDomain, assembly, targetNamespace);

      AppDomain.Unload(appDomain);
    }

    public void RunMember(Assembly assembly, MemberInfo member)
    {
      AppDomain appDomain = CreateAppDomain(assembly);
      CreateRunner("Member", appDomain, assembly, member);

      AppDomain.Unload(appDomain);
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
