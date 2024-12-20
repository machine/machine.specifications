using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery.BuiltIn
{
    class NestedTypesDiscoverySpecs : WithDiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_discover_the_sample_behavior = () =>
        {
            var discoveredSpec = results.SingleOrDefault(x =>
                "should_remember_that_true_is_true".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "NestedSpec".Equals(x.ClassName, StringComparison.Ordinal));

            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.ContextDisplayName.ShouldEqual("Parent NestedSpec");

            discoveredSpec.LineNumber.ShouldEqual(70);
            discoveredSpec.CodeFilePath.EndsWith("NestedSpecSample.cs", StringComparison.Ordinal);
        };
    }
}
