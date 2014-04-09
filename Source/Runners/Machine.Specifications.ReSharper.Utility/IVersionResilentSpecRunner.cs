using System.Collections.Generic;

namespace Machine.Specifications.Runner.Utility
{
    public interface IVersionResilentSpecRunner
    {
        void RunSpecAssemblies(IEnumerable<SpecAssemblyPath> specAssemblyPaths, ISpecificationRunListener listener, RunOptions options);
    }
}