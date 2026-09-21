using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
    class BehaviorsSpecs : WithDiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_pick_up_the_behavior = () =>
        {
            var discoveredSpec = results.SingleOrDefault(x =>
                "sample_behavior_test".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "BehaviorSampleSpec".Equals(x.ClassName, StringComparison.Ordinal));

            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(10);
            discoveredSpec.CodeFilePath.EndsWith("BehaviorSample.cs", StringComparison.Ordinal);
        };

        It should_pick_up_the_behavior_field_type_and_name = () =>
        {
            var discoveredSpec = results.SingleOrDefault(x =>
                "sample_behavior_test".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "BehaviorSampleSpec".Equals(x.ClassName, StringComparison.Ordinal));

            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.BehaviorFieldName.ShouldEqual("some_behavior");
            discoveredSpec.BehaviorFieldType.ShouldEqual("SampleSpecs.SampleBehavior");
        };
    }
}
