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
    readonly ISpecificationRunListener _internalListener;
    readonly RunOptions _options;

    public AppDomainRunner(ISpecificationRunListener listener, RunOptions options)
    {
      _internalListener = listener;
      _listener = new RemoteRunListener(listener);
      _options = options;
    }

    [SecuritySafeCritical]
    public void RunAssembly(Assembly assembly)
    {
      _internalListener.OnRunStart();

      InternalRunAssembly(assembly);

      _internalListener.OnRunEnd();
    }

    [SecuritySafeCritical]
    public void RunAssemblies(IEnumerable<Assembly> assemblies)
    {
      _internalListener.OnRunStart();

      assemblies.Each(x => InternalRunAssembly(x));

      _internalListener.OnRunEnd();
    }

    [SecuritySafeCritical]
    public void RunNamespace(Assembly assembly, string targetNamespace)
    {
      _internalListener.OnRunStart();
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunnerAndUnloadAppDomain("Namespace", appDomain, assembly, targetNamespace);

      _internalListener.OnRunEnd();
    }

    [SecuritySafeCritical]
    public void RunMember(Assembly assembly, MemberInfo member)
    {
      _internalListener.OnRunStart();
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunnerAndUnloadAppDomain("Member", appDomain, assembly, member);

      _internalListener.OnRunEnd();
    }

    [SecuritySafeCritical]
    void InternalRunAssembly(Assembly assembly)
    {
      AppDomain appDomain = CreateAppDomain(assembly);

      CreateRunnerAndUnloadAppDomain("Assembly", appDomain, assembly);
    }

    [SecuritySafeCritical]
    void CreateRunnerAndUnloadAppDomain(string runMethod, AppDomain appDomain, Assembly assembly, params object[] args)
    {
      string mspecAssemblyFilename = Path.Combine(Path.GetDirectoryName(assembly.Location), "Machine.Specifications.dll");

      var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFilename);

      var constructorArgs = new object[args.Length + 3];
      constructorArgs[0] = _listener;
      constructorArgs[1] = assembly;
      constructorArgs[2] = _options;
      Array.Copy(args, 0, constructorArgs, 3, args.Length);

      using (new SpecAssemblyResolver(assembly))
      {
        try
        {
          appDomain.CreateInstanceAndUnwrap(mspecAssemblyName.FullName,
                                            "Machine.Specifications.Runner.Impl.AppDomainRunner+" + runMethod + "Runner",
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
        finally
        {
          AppDomain.Unload(appDomain);
        }
      }
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
        try
        {
          var runner = new DefaultRunner(listener, options);
          runner.RunAssembly(assembly);
        }
        catch (Exception err)
        {
          listener.OnFatalError(new ExceptionResult(err));
        }
      }

      [SecurityCritical]
      public override object InitializeLifetimeService()
      {
        return null;
      }
    }

    public class NamespaceRunner : MarshalByRefObject
    {
      public NamespaceRunner(ISpecificationRunListener listener, Assembly assembly, RunOptions options, string targetNamespace)
      {
        try
        {
          var runner = new DefaultRunner(listener, options);
          runner.RunNamespace(assembly, targetNamespace);
        }
        catch (Exception err)
        {
          listener.OnFatalError(new ExceptionResult(err));
        }
      }

      [SecurityCritical]
      public override object InitializeLifetimeService()
      {
        return null;
      }
    }

    public class MemberRunner : MarshalByRefObject
    {
      public MemberRunner(ISpecificationRunListener listener, Assembly assembly, RunOptions options, MemberInfo memberInfo)
      {
        try
        {
          var runner = new DefaultRunner(listener, options);
          runner.RunMember(assembly, memberInfo);
        }
        catch (Exception err)
        {
          listener.OnFatalError(new ExceptionResult(err));
        }
      }

      [SecurityCritical]
      public override object InitializeLifetimeService()
      {
        return null;
      }
    }
  }
}
