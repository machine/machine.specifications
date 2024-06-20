using System;
using System.Linq;
using Example.Exceptions;
using Machine.Specifications.Factories;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Async Delegate Runner")]
    class when_running_async_specifications : RunnerSpecs
    {
        Establish context = () =>
        {
            AsyncSpecifications.establish_invoked = false;
            AsyncSpecifications.because_invoked = false;
            AsyncSpecifications.async_it_invoked = false;
            AsyncSpecifications.sync_it_invoked = false;
            AsyncSpecifications.cleanup_invoked = false;
        };

        Because of = () =>
            Run<AsyncSpecifications>();

        It should_call_establish = () =>
            AsyncSpecifications.establish_invoked.ShouldBeTrue();

        It should_call_because = () =>
            AsyncSpecifications.because_invoked.ShouldBeTrue();

        It should_call_async_spec = () =>
            AsyncSpecifications.async_it_invoked.ShouldBeTrue();

        It should_call_sync_spec = () =>
            AsyncSpecifications.sync_it_invoked.ShouldBeTrue();

        It should_call_cleanup = () =>
            AsyncSpecifications.cleanup_invoked.ShouldBeTrue();
    }

    [Subject("Async Delegate Runner")]
    class when_running_async_specifications_with_exceptions : RunnerSpecs
    {
        static ContextFactory factory;

        static Type specs;

        static Result[] results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            specs = typeof(AsyncSpecificationsWithExceptions);

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

        It should_have_exception_details = () =>
            results.ShouldEachConformTo(r => r.Exception.TypeName == nameof(InvalidOperationException) &&
                                                   r.Exception.Message == "something went wrong" &&
                                                   r.Exception.StackTrace.Contains(typeof(AsyncSpecificationsWithExceptions).FullName));
    }

    [Subject("Async Delegate Runner")]
    class when_running_async_value_task_specifications : RunnerSpecs
    {
        static Type specs;

        Establish context = () =>
        {
            specs = typeof(AsyncSpecificationsValueTask);

            AsyncSpecificationsValueTask.establish_value = 0;
            AsyncSpecificationsValueTask.because_value = 0;
            AsyncSpecificationsValueTask.async_it_value = 0;
            AsyncSpecificationsValueTask.sync_it_value = 0;
            AsyncSpecificationsValueTask.cleanup_value = 0;
        };

        Because of = () =>
            Run<AsyncSpecificationsValueTask>();

        It should_call_establish = () =>
            AsyncSpecificationsValueTask.establish_value.ShouldEqual(10);

        It should_call_because = () =>
            AsyncSpecificationsValueTask.because_value.ShouldEqual(10);

        It should_call_async_spec = () =>
            AsyncSpecificationsValueTask.async_it_value.ShouldEqual(10);

        It should_call_sync_spec = () =>
            AsyncSpecificationsValueTask.sync_it_value.ShouldEqual(10);

        It should_call_cleanup = () =>
            AsyncSpecificationsValueTask.cleanup_value.ShouldEqual(10);
    }
}
