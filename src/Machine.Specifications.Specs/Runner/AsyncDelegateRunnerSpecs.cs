using System;
using System.Linq;
using System.Threading.Tasks;
using Example.Random;
using FluentAssertions;
using Machine.Specifications.Factories;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Specs.Runner
{
    public class AsyncSpecificationsJustBecause
    {
        Because of = async () =>
            because_value = await GetThingAsync("because");

        It should_set_it_value_sync = () =>
            because_value.Should().Be("because");

        static async Task<string> GetThingAsync(string s)
        {
            await Task.Delay(10);
            return await Task.FromResult(s);
        }

        public static string because_value;
    }

    [Subject("Async Delegate Runner")]
    public class when_running_async_mixed_specifications : RunnerSpecs
    {
        Establish context = () =>
            AsyncSpecificationsJustBecause.because_value = "aa";

        Because of = () =>
            Run<AsyncSpecificationsJustBecause>();

        It should_set_because = () =>
            AsyncSpecificationsJustBecause.because_value.Should().Be("because");
    }

    [Subject("Async Delegate Runner")]
    public class when_running_async_specifications : RunnerSpecs
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
            AsyncSpecifications.establish_invoked.Should().BeTrue();

        It should_call_because = () =>
            AsyncSpecifications.because_invoked.Should().BeTrue();

        It should_call_async_spec = () =>
            AsyncSpecifications.async_it_invoked.Should().BeTrue();

        It should_call_sync_spec = () =>
            AsyncSpecifications.sync_it_invoked.Should().BeTrue();

        It should_call_cleanup = () =>
            AsyncSpecifications.cleanup_invoked.Should().BeTrue();
    }

    [Subject("Async Delegate Runner")]
    public class when_running_async_specifications_with_exceptions : RunnerSpecs
    {
        static ContextFactory factory;

        static Result[] results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            var context = factory.CreateContextFrom(Activator.CreateInstance<AsyncSpecificationsWithExceptions>());

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
            results.Length.Should().Be(2);

        It should_have_failures = () =>
            results.Should().Match(x => x.All(y => !y.Passed));
    }
}
