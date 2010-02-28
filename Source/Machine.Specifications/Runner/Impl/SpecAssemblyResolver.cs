using System;
using System.Reflection;

namespace Machine.Specifications.Runner.Impl
{
  public class SpecAssemblyResolver : IDisposable
  {
    readonly Assembly _assembly;

    public SpecAssemblyResolver(Assembly assembly)
    {
      _assembly = assembly;
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

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