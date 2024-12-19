using System.Collections.Generic;

namespace Machine.Specifications.Runner.VisualStudio.Discovery
{
    public interface ISpecificationDiscoverer
    {
        IEnumerable<SpecTestCase> DiscoverSpecs(string assemblyPath);
    }
}
