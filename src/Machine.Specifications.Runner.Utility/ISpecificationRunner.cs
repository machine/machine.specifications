using System.Collections.Generic;
using System.Reflection;

namespace Machine.Specifications.Runner.Utility
{
    public interface ISpecificationRunner
    {
        void RunAssembly(AssemblyPath assembly);
        void RunAssemblies(IEnumerable<AssemblyPath> assemblies);
        void RunNamespace(AssemblyPath assembly, string targetNamespace);
        void RunMember(AssemblyPath assembly, MemberInfo member);
        void StartRun(AssemblyPath assembly);
        void EndRun(AssemblyPath assembly);
    }
}