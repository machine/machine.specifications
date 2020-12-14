using System.Collections.Generic;
using System.Reflection;

namespace Machine.Specifications.Runner.Utility
{
    public class NullSpecificationRunner : ISpecificationRunner
    {
        public void RunAssembly(AssemblyPath assembly)
        {
        }

        public void RunAssemblies(IEnumerable<AssemblyPath> assemblies)
        {
        }

        public void RunNamespace(AssemblyPath assembly, string targetNamespace)
        {
        }

        public void RunMember(AssemblyPath assembly, MemberInfo member)
        {
        }

        public void StartRun(AssemblyPath assembly)
        {
        }

        public void EndRun(AssemblyPath assembly)
        {
        }
    }
}
