using System;
using System.Diagnostics;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Integration.AppVeyor
{
    public class AppVeyorReporter
    {
        private const string FrameworkName = "Machine.Specifications";

        private readonly Action<string> writer;

        private readonly IAppVeyorBuildWorkerApiClient apiClient;

        private readonly Stopwatch specificationTimer = new Stopwatch();

        private AssemblyInfo currentAssembly;

        private string currentContext;

        private bool failureOccurred;

        public AppVeyorReporter(Action<string> writer, IAppVeyorBuildWorkerApiClient apiClient)
        {
            this.writer = writer;
            this.apiClient = apiClient;
            failureOccurred = false;
        }

        public bool FailureOccurred => failureOccurred;

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
            writer(string.Empty);
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            writer($"\nSpecs in {assembly.Name}:");
            currentAssembly = assembly;
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnContextStart(ContextInfo context)
        {
            writer($"\n{context.FullName}");
            currentContext = context.FullName;
        }

        public void OnContextEnd(ContextInfo context)
        {
            currentContext = string.Empty;
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            specificationTimer.Reset();
            specificationTimer.Start();

            apiClient.AddTest(
                GetSpecificationName(specification),
                FrameworkName,
                currentAssembly.Name,
                "Running",
                null,
                null,
                null,
                null,
                null);
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            specificationTimer.Stop();

            var duration = specificationTimer.ElapsedMilliseconds;

            switch (result.Status)
            {
                case Status.Passing:
                    UpdateTestPassed(specification, duration);
                    break;

                case Status.NotImplemented:
                    UpdateTestNotImplemented(specification, duration);
                    break;

                case Status.Ignored:
                    UpdateTestIgnored(specification, duration);
                    break;

                default:
                    UpdateTestFailed(specification, duration, result.Exception);
                    failureOccurred = true;
                    break;
            }
        }

        private void UpdateTestPassed(SpecificationInfo specification, long executionTime)
        {
            writer($"\x1B[32m* {specification.Name}\x1B[39m"); // green
            apiClient.UpdateTest(
                GetSpecificationName(specification),
                FrameworkName,
                currentAssembly.Name,
                "Passed",
                executionTime,
                null,
                null,
                specification.CapturedOutput,
                null);
        }

        private void UpdateTestFailed(SpecificationInfo specification, long executionTime, ExceptionResult exceptionResult = null)
        {
            writer($"\x1B[31m* {specification.Name} (FAILED)\x1B[39m"); // red

            string errorMessage = null;
            string errorStackTrace = null;

            if (exceptionResult != null)
            {
                errorMessage = exceptionResult.Message;
                errorStackTrace = exceptionResult.ToString();
            }

            apiClient.UpdateTest(
                GetSpecificationName(specification),
                FrameworkName,
                currentAssembly.Name,
                "Failed",
                executionTime,
                errorMessage,
                errorStackTrace,
                specification.CapturedOutput,
                null);
        }

        private void UpdateTestIgnored(SpecificationInfo specification, long executionTime)
        {
            writer($"\x1B[33m* {specification.Name} (IGNORED)\x1B[39m"); // yellow
            apiClient.UpdateTest(
                GetSpecificationName(specification),
                FrameworkName,
                currentAssembly.Name,
                "Skipped",
                executionTime,
                null,
                null,
                null,
                null);
        }

        private void UpdateTestNotImplemented(SpecificationInfo specification, long executionTime)
        {
            writer($"\x1B[90m* {specification.Name} (NOT IMPLEMENTED)\x1B[39m"); // gray
            apiClient.UpdateTest(
                GetSpecificationName(specification),
                FrameworkName,
                currentAssembly.Name,
                "NotImplemented",
                executionTime,
                null,
                null,
                null,
                null);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            failureOccurred = true;
        }

        protected string GetSpecificationName(SpecificationInfo specification)
        {
            return currentContext + " » " + specification.Name;
        }
    }
}
