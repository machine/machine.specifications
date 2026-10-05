using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
    class CustomActAssertSpecs : WithDiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_find_some = () =>
        {
            var discoveredSpec = results.SingleOrDefault(x =>
                "should_have_the_same_hash_code".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "CustomActAssertDelegateSpec".Equals(x.ClassName, StringComparison.Ordinal));

            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(63);
            discoveredSpec.CodeFilePath.EndsWith("CustomActAssertDelegateSpec.cs", StringComparison.Ordinal);
        };
    }
}
