using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Specs.Runner
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Specifications.Runner.Utility;
    using Sdk;
    using AssemblyInfo = Specifications.Runner.AssemblyInfo;
    using ContextInfo = Specifications.Runner.ContextInfo;
    using ExceptionResult = ExceptionResult;
    using Result = Result;
    using RunnerAssemblyInfo = AssemblyInfo;
    using RunnerContextInfo = ContextInfo;
    using RunnerExceptionResult = Specifications.Runner.Utility.ExceptionResult;
    using RunnerResult = Specifications.Runner.Utility.Result;
    using RunnerSpecificationInfo = Specifications.Runner.Utility.SpecificationInfo;
    using SpecificationInfo = Specifications.Runner.SpecificationInfo;

    public class remote_run
    {
        protected static RemoteToInternalSpecificationRunListenerAdapter adapter;

        protected static RunnerAssemblyInfo assemblyStart;
        protected static RunnerAssemblyInfo assemblyEnd;

        protected static RunnerSpecificationInfo specificationEnd;
        protected static RunnerSpecificationInfo specificationStart;
        protected static RunnerResult specificationEndResult;

        protected static RunnerContextInfo contextStart;

        protected static RunnerContextInfo contextEnd;

        protected static bool runStart;

        protected static bool runEnd;

        protected static RunnerExceptionResult fatalError;

        Establish ctx = () =>
            {
                var remoteListener = new Listener();
                adapter = new TestableRemoteToInternalSpecificationRunListenerAdapter(remoteListener);
            };

        Cleanup cleanup = () =>
            {
                assemblyStart = null;
                assemblyEnd = null;
                specificationEnd = null;
                specificationStart = null;
                specificationEndResult = null;
                contextStart = null;
                contextEnd = null;
                fatalError = null;
                runStart = false;
                runEnd = false;
            };

        private class TestableRemoteToInternalSpecificationRunListenerAdapter : RemoteToInternalSpecificationRunListenerAdapter
        {
            public TestableRemoteToInternalSpecificationRunListenerAdapter(object listener)
                : base(listener, Specifications.Runner.Utility.RunOptions.Default.ToXml())
            {
            }
        }

        private class Listener : ISpecificationRunListener
        {
            public void OnAssemblyStart(RunnerAssemblyInfo assemblyInfo)
            {
                assemblyStart = assemblyInfo;
            }

            public void OnAssemblyEnd(RunnerAssemblyInfo assemblyInfo)
            {
                assemblyEnd = assemblyInfo;
            }

            public void OnSpecificationEnd(RunnerSpecificationInfo specificationInfo, RunnerResult result)
            {
                specificationEnd = specificationInfo;
                specificationEndResult = result;
            }

            public void OnSpecificationStart(RunnerSpecificationInfo specificationInfo)
            {
                specificationStart = specificationInfo;
            }

            public void OnContextStart(RunnerContextInfo contextInfo)
            {
                contextStart = contextInfo;
            }

            public void OnContextEnd(RunnerContextInfo contextInfo)
            {
                contextEnd = contextInfo;
            }

            public void OnRunStart()
            {
                runStart = true;
            }

            public void OnRunEnd()
            {
                runEnd = true;
            }

            public void OnFatalError(RunnerExceptionResult exceptionResult)
            {
                fatalError = exceptionResult;
            }
        }
    }

    public class when_remote_run_listener_observes_a_failed_run : remote_run
    {
        static AssemblyInfo assemblyInfo;

        static SpecificationInfo specificationInfo;

        static ExceptionResult exceptionResult;

        static Result result;

        static ContextInfo contexInfo;

        Establish ctx = () =>
            {
                assemblyInfo = new AssemblyInfo("assembly", "location");
                specificationInfo = new SpecificationInfo("leader", "name", "containingType", "fieldName");
                exceptionResult = new ExceptionResult(new InvalidOperationException("Argf", new ArgumentException()));
                result = Result.Failure(new InvalidOperationException("foo", new ArgumentException()));
                result.Supplements.Add("Foo", new Dictionary<string, string>
                                               {
                                                   { "Foo", "Bar" },
                                                   { "Bar", "Foo" },
                                               });
                result.Supplements.Add("Bar", new Dictionary<string, string>
                                               {
                                                   { "Bar", "Foo" },
                                                   { "", "Foo" },
                                               });
                contexInfo = new ContextInfo("name", "concern", "typeName", "namespace", "assemblyname");
            };

        Because of = () => adapter.Run(assemblyInfo, specificationInfo, result, exceptionResult, contexInfo);

        It should_run_start = () => runStart.Should().BeTrue();
        It should_run_end = () => runEnd.Should().BeTrue();
        It should_context_start = () => contextStart.ShouldBeEquivalentTo(contexInfo);
        It should_context_end = () => contextEnd.ShouldBeEquivalentTo(contexInfo);
        It should_fatal_error = () => fatalError.ShouldBeEquivalentTo(exceptionResult, c => c.IncludingNestedObjects());
        It should_specification_start = () => specificationStart.ShouldBeEquivalentTo(specificationInfo);
        It should_specification_end = () => specificationEnd.ShouldBeEquivalentTo(specificationInfo);
        It should_specification_end_result = () => specificationEndResult.ShouldBeEquivalentTo(result, c => c.IncludingNestedObjects().Excluding(r => r.Status));
        It should_assembly_start = () => assemblyStart.ShouldBeEquivalentTo(assemblyInfo);
        It should_assembly_end = () => assemblyEnd.ShouldBeEquivalentTo(assemblyInfo);
    }

    public class when_remote_run_listener_observes_a_successful_run : remote_run
    {
        static AssemblyInfo assemblyInfo;

        static SpecificationInfo specificationInfo;

        static ExceptionResult exceptionResult;

        static Result result;

        static ContextInfo contexInfo;

        Establish ctx = () =>
        {
            exceptionResult = null; // empty exception result
            assemblyInfo = new AssemblyInfo("assembly", "location");
            specificationInfo = new SpecificationInfo("leader", "name", "containingType", "fieldName");
            result = Result.Pass();
            contexInfo = new ContextInfo("name", "concern", "typeName", "namespace", "assemblyname");
        };

        Because of = () => adapter.Run(assemblyInfo, specificationInfo, result, exceptionResult, contexInfo);

        It should_run_start = () => runStart.Should().BeTrue();
        It should_run_end = () => runEnd.Should().BeTrue();
        It should_context_start = () => contextStart.ShouldBeEquivalentTo(contexInfo);
        It should_context_end = () => contextEnd.ShouldBeEquivalentTo(contexInfo);
        It should_fatal_error = () => fatalError.Should().BeNull();
        It should_specification_start = () => specificationStart.ShouldBeEquivalentTo(specificationInfo);
        It should_specification_end = () => specificationEnd.ShouldBeEquivalentTo(specificationInfo);
        It should_specification_end_result = () => specificationEndResult.ShouldBeEquivalentTo(result, c => c.IncludingNestedObjects().Excluding(r => r.Status));
        It should_assembly_start = () => assemblyStart.ShouldBeEquivalentTo(assemblyInfo);
        It should_assembly_end = () => assemblyEnd.ShouldBeEquivalentTo(assemblyInfo);
    }

    static class RemoteToInternalSpecificationRunListenerAdapterExtensions
    {
        public static void Run(this RemoteToInternalSpecificationRunListenerAdapter adapter, AssemblyInfo assemblyInfo, SpecificationInfo specificationInfo, Result failure, ExceptionResult exceptionResult, ContextInfo contexInfo)
        {
            adapter.OnAssemblyStart(assemblyInfo);
            adapter.OnAssemblyEnd(assemblyInfo);

            adapter.OnSpecificationStart(specificationInfo);
            adapter.OnSpecificationEnd(specificationInfo, failure);

            adapter.OnFatalError(exceptionResult);

            adapter.OnContextStart(contexInfo);
            adapter.OnContextEnd(contexInfo);

            adapter.OnRunStart();
            adapter.OnRunEnd(); 
        }
    }
}