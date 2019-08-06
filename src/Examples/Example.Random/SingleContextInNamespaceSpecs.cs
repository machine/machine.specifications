using Machine.Specifications;

namespace Example.Random.SingleContextInThisNamespace
{
    public class context_without_any_other_in_the_same_namespace
    {
        Establish context = () => { };

        Because of = () => { };

        It spec1 = () => { };

        It spec2 = () => { };
    }
}
