using System.Collections.Generic;

namespace Machine.Specifications.Runner.Utility
{
    public interface ISpecificationRunner
    {
        void RunAssemblies(IEnumerable<AssemblyPath> assemblyPaths, ISpecificationRunListener listener, RunOptions options);
    }
}