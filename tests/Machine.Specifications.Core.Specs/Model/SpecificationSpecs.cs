using System;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Specs.Runner;

namespace Machine.Specifications.Specs.Model
{
    [Subject(typeof(Specification))]
    class SpecificationSpecs : RunnerSpecs
    {
        static ContextFactory factory;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            ContextWithSingleSpecification.Reset();

            var context = factory.CreateContextFrom(new ContextWithSingleSpecification());

            ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_establish_context = () =>
            ContextWithSingleSpecification.because_invoked.ShouldBeTrue();

        It should_call_before_each = () =>
            ContextWithSingleSpecification.context_invoked.ShouldBeTrue();

        It should_cleanup = () =>
            ContextWithSingleSpecification.cleanup_invoked.ShouldBeTrue();
    }
}
