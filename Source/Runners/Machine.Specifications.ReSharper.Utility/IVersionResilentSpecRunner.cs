using System;
using System.Collections.Generic;

namespace Machine.Specifications.Runner.Utility
{
    public interface IVersionResilentSpecRunner : IDisposable
    {
        void RunSpecs(string specAssemblyName, ISpecificationRunListener listener, IEnumerable<string> contextList);

        void RunSpecAssemblies(IEnumerable<string> testAssemblyNames, ISpecificationRunListener listener, RemoteRunOptions options);

        void RunSpecAssembly(string specAssemblyName, ISpecificationRunListener listener, RemoteRunOptions options);
    }
}