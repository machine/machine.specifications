using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Specs.Model
{
    [Subject(typeof(Context))]
    public class ExpectingThrowButDoesntTests
    {
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
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
            results.First().Exception.Should().NotBeNull();

        It should_fail = () =>
            results.First().Passed.Should().BeFalse();
    }

    [Subject(typeof(Context))]
    public class ThrowingWhenTests
    {
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
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
            ContextWithThrowingWhenAndPassingSpecification.ItInvoked.Should().BeFalse();

        It should_fail = () =>
            results.First().Passed.Should().BeFalse();
    }

    [Subject(typeof(Context))]
    public class EmptyContextTests
    {
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
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
            ContextWithEmptyWhen.ItInvoked.Should().BeTrue();
    }

    [Subject(typeof(Context))]
    public class EmptySpecificationTests
    {
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
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
            ContextWithEmptySpecification.WhenInvoked.Should().BeFalse();

        It should_have_not_implemented_result = () =>
            results.First().Status.Should().Be(Status.NotImplemented);

        It should_have_failed_result = () =>
            results.First().Passed.Should().BeFalse();
    }

    [Subject(typeof(Context))]
    public class ContextTests
    {
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
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
            ContextWithSingleSpecification.BecauseInvoked.Should().BeTrue();

        It should_call_before_each = () =>
            ContextWithSingleSpecification.ContextInvoked.Should().BeTrue();

        It should_cleanup = () =>
            ContextWithSingleSpecification.CleanupInvoked.Should().BeTrue();
    }
}
