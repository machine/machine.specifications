using System;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Specs.Runner;

namespace Machine.Specifications.Specs.Model
{
    [Subject(typeof(Specification))]
    public class SpecificationSpecs : RandomRunnerSpecs
    {
        static Type ContextWithSingleSpecification;
        static ContextFactory factory;

        Establish context = () =>
        {
            ContextWithSingleSpecification = GetFramework("ContextWithSingleSpecification");

            factory = new ContextFactory();
        };

        Because of = () =>
        {
            ContextWithSingleSpecification.ToDynamic().Reset();

            var context = factory.CreateContextFrom(Activator.CreateInstance(ContextWithSingleSpecification));

            ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_establish_context = () =>
            ContextWithSingleSpecification.ToDynamic().because_invoked.ShouldBeTrue();

        It should_call_before_each = () =>
            ContextWithSingleSpecification.ToDynamic().context_invoked.ShouldBeTrue();

        It should_cleanup = () =>
            ContextWithSingleSpecification.ToDynamic().cleanup_invoked.ShouldBeTrue();
    }
}
