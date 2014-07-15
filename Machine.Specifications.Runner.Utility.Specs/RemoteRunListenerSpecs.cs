using System;
using System.Collections.Generic;
using FluentAssertions;
using Machine.Specifications.Sdk;

namespace Machine.Specifications.Runner.Utility
{
    public class remote_run
    {
        protected static RemoteToInternalSpecificationRunListenerAdapter adapter;

        protected static AssemblyInfo assemblyStart;
        protected static AssemblyInfo assemblyEnd;

        protected static SpecificationInfo specificationEnd;
        protected static SpecificationInfo specificationStart;
        protected static Result specificationEndResult;

        protected static ContextInfo contextStart;

        protected static ContextInfo contextEnd;

        protected static bool runStart;

        protected static bool runEnd;

        protected static ExceptionResult fatalError;

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
                : base(listener, Utility.RunOptions.Default.ToXml())
            {
            }
        }

        private class Listener : ISpecificationRunListener
        {
            public void OnAssemblyStart(AssemblyInfo assemblyInfo)
            {
                assemblyStart = assemblyInfo;
            }

            public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
            {
                assemblyEnd = assemblyInfo;
            }

            public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
            {
                specificationEnd = specificationInfo;
                specificationEndResult = result;
            }

            public void OnSpecificationStart(SpecificationInfo specificationInfo)
            {
                specificationStart = specificationInfo;
            }

            public void OnContextStart(ContextInfo contextInfo)
            {
                contextStart = contextInfo;
            }

            public void OnContextEnd(ContextInfo contextInfo)
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

            public void OnFatalError(ExceptionResult exceptionResult)
            {
                fatalError = exceptionResult;
            }
        }
    }

    public class when_remote_run_listener_observes_a_failed_run : remote_run
    {
        static Runner.AssemblyInfo assemblyInfo;

        static Runner.SpecificationInfo specificationInfo;

        static Specifications.ExceptionResult exceptionResult;

        static Specifications.Result result;

        static Runner.ContextInfo contexInfo;

        Establish ctx = () =>
            {
                assemblyInfo = new Runner.AssemblyInfo("assembly", "location");
                specificationInfo = new Runner.SpecificationInfo("leader", "name", "containingType", "fieldName");
                exceptionResult = new Specifications.ExceptionResult(new InvalidOperationException("Argf", new ArgumentException()));
                result = Specifications.Result.Failure(new InvalidOperationException("foo", new ArgumentException()));
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
                contexInfo = new Runner.ContextInfo("name", "concern", "typeName", "namespace", "assemblyname");
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
        static Runner.AssemblyInfo assemblyInfo;

        static Runner.SpecificationInfo specificationInfo;

        static Specifications.ExceptionResult exceptionResult;

        static Specifications.Result result;

        static Runner.ContextInfo contexInfo;

        Establish ctx = () =>
        {
            exceptionResult = null; // empty exception result
            assemblyInfo = new Runner.AssemblyInfo("assembly", "location");
            specificationInfo = new Runner.SpecificationInfo("leader", "name", "containingType", "fieldName");
            result = Specifications.Result.Pass();
            contexInfo = new Runner.ContextInfo("name", "concern", "typeName", "namespace", "assemblyname");
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
        public static void Run(this RemoteToInternalSpecificationRunListenerAdapter adapter, Runner.AssemblyInfo assemblyInfo, Runner.SpecificationInfo specificationInfo, Specifications.Result failure, Specifications.ExceptionResult exceptionResult, Runner.ContextInfo contexInfo)
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