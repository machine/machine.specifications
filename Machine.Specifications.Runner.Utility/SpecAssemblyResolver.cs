using System;
using System.Reflection;
using System.Security;

namespace Machine.Specifications.Runner.Utility
{
    internal class SpecAssemblyResolver : IDisposable
    {
        readonly AssemblyPath _assembly;

        [SecuritySafeCritical]
        public SpecAssemblyResolver(AssemblyPath assembly)
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
            if (args.Name == _assembly)
            {
                return Assembly.LoadFile(_assembly);
            }
            return null;
        }
    }
}