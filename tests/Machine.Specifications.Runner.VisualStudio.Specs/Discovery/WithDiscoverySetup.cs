using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
    public abstract class WithDiscoverySetup<TDiscoverer>
        where TDiscoverer : ISpecificationDiscoverer, new()
    {
        static ISpecificationDiscoverer discoverer;

        static Assembly assembly;

        protected static IEnumerable<SpecTestCase> results;

        Establish context = () =>
        {
            discoverer = new TDiscoverer();

            assembly = typeof(StandardSpec).Assembly;
        };

        Because of = () =>
            results = discoverer.DiscoverSpecs(assembly.Location);
    }
}
