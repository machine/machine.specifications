using Machine.Specifications;

namespace Example.Random
{
    [Behaviors]
    public class SampleBehaviors
    {
        It behavior1 = () => { };
    }

    public class context_with_specs_and_behaviors
    {
        Establish context = () => { };

        Because of = () => { };

        Behaves_like<SampleBehaviors> behaviors;

        It spec1 = () => { };
        It spec2 = () => { };
    }
}