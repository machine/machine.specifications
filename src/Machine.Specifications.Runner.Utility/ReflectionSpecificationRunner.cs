using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Runner.Utility
{
    public class ReflectionSpecificationRunner : ISpecificationRunner
    {
        private readonly Runner.ISpecificationRunner runner;

        public ReflectionSpecificationRunner(Runner.ISpecificationRunner runner)
        {
            this.runner = runner;
        }

        public void RunAssembly(AssemblyPath assembly)
        {
            runner.RunAssembly(Assembly.LoadFrom(assembly));
        }

        public void RunAssemblies(IEnumerable<AssemblyPath> assemblies)
        {
            var assembliesList = assemblies
                .Select(x => Assembly.LoadFrom(x))
                .ToArray();

            runner.RunAssemblies(assembliesList);
        }

        public void RunNamespace(AssemblyPath assembly, string targetNamespace)
        {
            runner.RunNamespace(Assembly.LoadFrom(assembly), targetNamespace);
        }

        public void RunMember(AssemblyPath assembly, MemberInfo member)
        {
            runner.RunMember(Assembly.LoadFrom(assembly), member);
        }

        public void StartRun(AssemblyPath assembly)
        {
            runner.StartRun(Assembly.LoadFrom(assembly));
        }

        public void EndRun(AssemblyPath assembly)
        {
            runner.EndRun(Assembly.LoadFrom(assembly));
        }
    }
}
