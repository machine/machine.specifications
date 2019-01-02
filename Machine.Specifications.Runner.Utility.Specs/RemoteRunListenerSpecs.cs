using System;
using System.Collections.Generic;
using FluentAssertions;

namespace Machine.Specifications.Runner.Utility
{
    public class remote_run
    {
        protected static ISpecificationRunListener adapter;

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
                adapter = new RemoteRunListener(remoteListener);
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
        It should_context_start = () => contextStart.Should().BeEquivalentTo(contexInfo);
        It should_context_end = () => contextEnd.Should().BeEquivalentTo(contexInfo);
        It should_fatal_error = () => fatalError.Should().BeEquivalentTo(exceptionResult, c => c.IncludingNestedObjects());
        It should_specification_start = () => specificationStart.Should().BeEquivalentTo(specificationInfo);
        It should_specification_end = () => specificationEnd.Should().BeEquivalentTo(specificationInfo);
        It should_specification_end_result = () => specificationEndResult.Should().BeEquivalentTo(result, c => c.IncludingNestedObjects().Excluding(r => r.Status));
        It should_assembly_start = () => assemblyStart.Should().BeEquivalentTo(assemblyInfo);
        It should_assembly_end = () => assemblyEnd.Should().BeEquivalentTo(assemblyInfo);
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
        It should_context_start = () => contextStart.Should().BeEquivalentTo(contexInfo);
        It should_context_end = () => contextEnd.Should().BeEquivalentTo(contexInfo);
        It should_fatal_error = () => fatalError.Should().BeNull();
        It should_specification_start = () => specificationStart.Should().BeEquivalentTo(specificationInfo);
        It should_specification_end = () => specificationEnd.Should().BeEquivalentTo(specificationInfo);
        It should_specification_end_result = () => specificationEndResult.Should().BeEquivalentTo(result, c => c.IncludingNestedObjects().Excluding(r => r.Status));
        It should_assembly_start = () => assemblyStart.Should().BeEquivalentTo(assemblyInfo);
        It should_assembly_end = () => assemblyEnd.Should().BeEquivalentTo(assemblyInfo);
    }

    static class RemoteToInternalSpecificationRunListenerAdapterExtensions
    {
        public static void Run(this ISpecificationRunListener adapter, AssemblyInfo assemblyInfo, SpecificationInfo specificationInfo, Result failure, ExceptionResult exceptionResult, ContextInfo contexInfo)
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