using System;
using System.Collections.Generic;

namespace Machine.Specifications.Runner.Utility
{
    using Machine.Specifications.Runner.Utility.SpecsRunner;

    public interface IVersionResilentSpecRunner : IDisposable
    {
        void RunSpecs(string specAssemblyName, IRemoteSpecificationRunListener listener, IEnumerable<string> contextList);

        void RunSpecAssemblies(IEnumerable<string> testAssemblyNames, IRemoteSpecificationRunListener listener, RemoteRunOptions options);

        void RunSpecAssembly(string specAssemblyName, IRemoteSpecificationRunListener listener, RemoteRunOptions options);
    }
}