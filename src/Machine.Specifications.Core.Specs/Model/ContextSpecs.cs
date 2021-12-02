using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Specs.Runner;

namespace Machine.Specifications.Specs.Model
{
    [Subject(typeof(Context))]
    class ExpectingThrowButDoesntTests : RunnerSpecs
    {
        static ContextFactory factory;

        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            ContextWithSpecificationExpectingThrowThatDoesnt.Reset();

            var context = factory.CreateContextFrom(new ContextWithSpecificationExpectingThrowThatDoesnt());

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_have_exception = () =>
            results.First().Exception.ShouldNotBeNull();

        It should_fail = () =>
            results.First().Passed.ShouldBeFalse();
    }

    [Subject(typeof(Context))]
    class ThrowingWhenTests : RunnerSpecs
    {
        static ContextFactory factory;

        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            ContextWithThrowingWhenAndPassingSpecification.Reset();

            var context = factory.CreateContextFrom(new ContextWithThrowingWhenAndPassingSpecification());

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_not_call_id = () =>
            ContextWithThrowingWhenAndPassingSpecification.it_invoked.ShouldBeFalse();

        It should_fail = () =>
            results.First().Passed.ShouldBeFalse();
    }

    [Subject(typeof(Context))]
    class EmptyContextTests : RunnerSpecs
    {
        static ContextFactory factory;

        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            ContextWithEmptyWhen.Reset();

            var context = factory.CreateContextFrom(new ContextWithEmptyWhen());

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_call_it = () =>
            ContextWithEmptyWhen.it_invoked.ShouldBeTrue();
    }

    [Subject(typeof(Context))]
    class EmptySpecificationTests : RunnerSpecs
    {
        static ContextFactory factory;

        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            ContextWithEmptySpecification.Reset();

            var context = factory.CreateContextFrom(new ContextWithEmptySpecification());

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_not_call_when = () =>
            ContextWithEmptySpecification.when_invoked.ShouldBeFalse();

        It should_have_not_implemented_result = () =>
            results.First().Status.ShouldEqual(Status.NotImplemented);

        It should_have_failed_result = () =>
            results.First().Passed.ShouldBeFalse();
    }

    [Subject(typeof(Context))]
    class ContextTests : RunnerSpecs
    {
        static ContextFactory factory;

        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            ContextWithSingleSpecification.Reset();

            var context = factory.CreateContextFrom(new ContextWithSingleSpecification());

            results = ContextRunnerFactory
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
