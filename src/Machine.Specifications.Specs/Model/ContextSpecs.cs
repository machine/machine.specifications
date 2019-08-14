using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Specs.Runner;

namespace Machine.Specifications.Specs.Model
{
    [Subject(typeof(Context))]
    public class ExpectingThrowButDoesntTests : RandomRunnerSpecs
    {
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        private Because of = () =>
        {
            var type = Assembly
                .LoadFile(AssemblyPath)
                .GetType("Machine.Specifications.ContextWithSpecificationExpectingThrowThatDoesnt");

            var context = factory.CreateContextFrom(Activator.CreateInstance(type));

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
    public class ThrowingWhenTests : RandomRunnerSpecs
    {
        static Type type;
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            type = Assembly
                .LoadFile(AssemblyPath)
                .GetType("Machine.Specifications.ContextWithThrowingWhenAndPassingSpecification");

            var context = factory.CreateContextFrom(Activator.CreateInstance(type));

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_not_call_id = () =>
            type.ToDynamic().it_invoked.ShouldBeFalse();

        It should_fail = () =>
            results.First().Passed.ShouldBeFalse();
    }

    [Subject(typeof(Context))]
    public class EmptyContextTests : RandomRunnerSpecs
    {
        static Type type;
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            type = Assembly
                .LoadFile(AssemblyPath)
                .GetType("Machine.Specifications.ContextWithEmptyWhen");

            var context = factory.CreateContextFrom(Activator.CreateInstance(type));

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_call_it = () =>
            type.ToDynamic().it_invoked.ShouldBeTrue();
    }

    [Subject(typeof(Context))]
    public class EmptySpecificationTests : RandomRunnerSpecs
    {
        static Type type;
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            type = Assembly
                .LoadFile(AssemblyPath)
                .GetType("Machine.Specifications.ContextWithEmptySpecification");

            var context = factory.CreateContextFrom(Activator.CreateInstance(type));

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_not_call_when = () =>
            type.ToDynamic().when_invoked.ShouldBeFalse();

        It should_have_not_implemented_result = () =>
            results.First().Status.ShouldEqual(Status.NotImplemented);

        It should_have_failed_result = () =>
            results.First().Passed.ShouldBeFalse();
    }

    [Subject(typeof(Context))]
    public class ContextTests : RandomRunnerSpecs
    {
        static Type type;
        static ContextFactory factory;
        static IEnumerable<Result> results;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            type = Assembly
                .LoadFile(AssemblyPath)
                .GetType("Machine.Specifications.ContextWithSingleSpecification");

            var context = factory.CreateContextFrom(Activator.CreateInstance(type));

            results = ContextRunnerFactory
                .GetContextRunnerFor(context)
                .Run(context,
                    new RunListenerBase(),
                    RunOptions.Default,
                    Array.Empty<ICleanupAfterEveryContextInAssembly>(),
                    Array.Empty<ISupplementSpecificationResults>());
        };

        It should_establish_context = () =>
            type.ToDynamic().because_invoked.ShouldBeTrue();

        It should_call_before_each = () =>
            type.ToDynamic().context_invoked.ShouldBeTrue();

        It should_cleanup = () =>
            type.ToDynamic().cleanup_invoked.ShouldBeTrue();
    }
}
