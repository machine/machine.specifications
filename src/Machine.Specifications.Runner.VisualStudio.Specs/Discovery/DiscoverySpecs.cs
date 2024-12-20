using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
    class DiscoverySpecs : WithDiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_find_spec = () =>
        {
            var discoveredSpec = results.SingleOrDefault(x =>
                "should_pass".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));

            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(79);
            discoveredSpec.CodeFilePath.EndsWith("StandardSpec.cs", StringComparison.Ordinal);
        };

        It should_find_empty_spec = () =>
        {
            var discoveredSpec = results.SingleOrDefault(x =>
                "should_be_ignored".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));

            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(83);
            discoveredSpec.CodeFilePath.EndsWith("StandardSpec.cs", StringComparison.Ordinal);
        };

        It should_find_ignored_spec_but_will_not_find_line_number = () =>
        {
            var discoveredSpec = results.SingleOrDefault(x =>
                "not_implemented".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));

            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(0);
            discoveredSpec.CodeFilePath.ShouldBeNull();
        };
    }
}
