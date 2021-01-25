using System;
using System.Linq;
using Machine.Specifications.Factories;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Async Delegate Runner")]
    public class when_running_async_specifications : RandomRunnerSpecs
    {
        static Type specs;

        Establish context = () =>
        {
            specs = GetFramework("AsyncSpecifications");

            specs.ToDynamic().because_invoked = false;
            specs.ToDynamic().async_it_invoked = false;
            specs.ToDynamic().sync_it_invoked = false;
        };

        Because of = () =>
            Run(specs);

        It should_call_because = () =>
            specs.ToDynamic().because_invoked.ShouldBeTrue();

        It should_call_async_spec = () =>
            specs.ToDynamic().async_it_invoked.ShouldBeTrue();

        It should_call_sync_spec = () =>
            specs.ToDynamic().sync_it_invoked.ShouldBeTrue();
    }

    [Subject("Async Delegate Runner")]
    public class when_running_async_specifications_with_exceptions : RandomRunnerSpecs
    {
        static ContextFactory factory;

        static Type specs;

        static Result[] results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            specs = GetFramework("AsyncSpecificationsWithExceptions");

            var context = factory.CreateContextFrom(Activator.CreateInstance(specs));

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>())
                .ToArray();
        };

        It should_run_two_specs = () =>
            results.Length.ShouldEqual(2);

        It should_have_failures = () =>
            results.ShouldEachConformTo(x => !x.Passed);
    }
}
