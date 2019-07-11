using FluentAssertions;
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
            ContextWithSingleSpecification.BecauseInvoked.Should().BeTrue();

        It should_call_before_each = () =>
            ContextWithSingleSpecification.ContextInvoked.Should().BeTrue();

        It should_cleanup = () =>
            ContextWithSingleSpecification.CleanupInvoked.Should().BeTrue();
    }
}
