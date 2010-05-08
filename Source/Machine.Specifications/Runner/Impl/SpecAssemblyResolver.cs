using System;
using System.Reflection;
using System.Security;

namespace Machine.Specifications.Runner.Impl
{
  public class SpecAssemblyResolver : IDisposable
  {
    readonly Assembly _assembly;

    [SecuritySafeCritical]
    public SpecAssemblyResolver(Assembly assembly)
    {
      _assembly = assembly;
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    [SecuritySafeCritical]
    public void Dispose()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
    }

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      if (args.Name == _assembly.GetName().FullName)
      {
        return _assembly;
      }
      return null;
    }
  }
}