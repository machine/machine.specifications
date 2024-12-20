using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Helpers;

namespace Machine.Specifications.Runner.VisualStudio.Discovery
{
    public class BuiltInSpecificationDiscoverer : ISpecificationDiscoverer
    {
        public IEnumerable<SpecTestCase> DiscoverSpecs(string assemblyFilePath)
        {
#if NETFRAMEWORK
            using (var scope = new IsolatedAppDomainExecutionScope<TestDiscoverer>(assemblyFilePath))
            {
                var discoverer = scope.CreateInstance();
#else
                var discoverer = new TestDiscoverer();
#endif
                return discoverer.DiscoverTests(assemblyFilePath).ToList();
#if NETFRAMEWORK
            }
#endif
        }
    }
}
