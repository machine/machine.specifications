using System.Collections.Generic;
using System.Reflection;

namespace Machine.Specifications.Runner.Impl
{
    internal class NullRunner : ISpecificationRunner
    {
        public void RunAssembly(Assembly assembly)
        {
        }

        public void RunAssemblies(IEnumerable<Assembly> assemblies)
        {
        }

        public void RunNamespace(Assembly assembly, string targetNamespace)
        {
        }

        public void RunMember(Assembly assembly, MemberInfo member)
        {
        }

        public void StartRun(Assembly assembly)
        {
        }

        public void EndRun(Assembly assembly)
        {
        }
    }
}