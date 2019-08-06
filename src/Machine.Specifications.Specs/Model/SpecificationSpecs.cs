using Machine.Specifications.Factories;
using Machine.Specifications.Model;

namespace Machine.Specifications.Specs.Model
{
    [Subject(typeof(Specification))]
    public class SpecificationSpecs
    {
        static ContextFactory factory;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
            factory.CreateContextFrom(new ContextWithSingleSpecification());

        It should_establish_context = () =>
            ContextWithSingleSpecification.because_invoked.ShouldBeTrue();

        It should_call_before_each = () =>
            ContextWithSingleSpecification.context_invoked.ShouldBeTrue();

        It should_cleanup = () =>
            ContextWithSingleSpecification.cleanup_invoked.ShouldBeTrue();
    }
}
