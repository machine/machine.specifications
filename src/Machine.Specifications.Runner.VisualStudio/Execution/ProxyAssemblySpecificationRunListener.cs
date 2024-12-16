using System;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class ProxyAssemblySpecificationRunListener :
#if NETFRAMEWORK
                                                            MarshalByRefObject,
#endif
        ISpecificationRunListener, IFrameworkLogger
    {
        private readonly IFrameworkHandle frameworkHandle;

        private readonly string assemblyPath;

        private readonly Uri executorUri;

        private ContextInfo currentContext;

        private RunStats currentRunStats;

        public ProxyAssemblySpecificationRunListener(string assemblyPath, IFrameworkHandle frameworkHandle, Uri executorUri)
        {
            this.frameworkHandle = frameworkHandle ?? throw new ArgumentNullException(nameof(frameworkHandle));
            this.assemblyPath = assemblyPath ?? throw new ArgumentNullException(nameof(assemblyPath));
            this.executorUri = executorUri ?? throw new ArgumentNullException(nameof(executorUri));
        }

#if NETFRAMEWORK
        [System.Security.SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }
#endif

        public void OnFatalError(ExceptionResult exception)
        {
            if (currentRunStats != null)
            {
                currentRunStats.Stop();
                currentRunStats = null;
            }

            frameworkHandle.SendMessage(TestMessageLevel.Error,
                "Machine Specifications Visual Studio Test Adapter - Fatal error while executing test." +
                Environment.NewLine + exception);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            var testCase = ConvertSpecificationToTestCase(specification);
            frameworkHandle.RecordStart(testCase);
            currentRunStats = new RunStats();
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            if (currentRunStats != null)
            {
                currentRunStats.Stop();
            }

            var testCase = ConvertSpecificationToTestCase(specification);

            frameworkHandle.RecordEnd(testCase, MapSpecificationResultToTestOutcome(result));
            frameworkHandle.RecordResult(ConverResultToTestResult(testCase, result, currentRunStats));
        }

        public void OnContextStart(ContextInfo context)
        {
            currentContext = context;
        }

        public void OnContextEnd(ContextInfo context)
        {
            currentContext = null;
        }

        private TestCase ConvertSpecificationToTestCase(SpecificationInfo specification)
        {
            var vsTestId = specification.ToVisualStudioTestIdentifier(currentContext);

            return new TestCase(vsTestId.FullyQualifiedName, executorUri, assemblyPath)
            {
                DisplayName = $"{currentContext?.TypeName}.{specification.FieldName}",
            };
        }

        private static TestOutcome MapSpecificationResultToTestOutcome(Result result)
        {
            switch (result.Status)
            {
                case Status.Failing:
                    return TestOutcome.Failed;

                case Status.Passing:
                    return TestOutcome.Passed;

                case Status.Ignored:
                    return TestOutcome.Skipped;

                case Status.NotImplemented:
                    return TestOutcome.NotFound;

                default:
                    return TestOutcome.None;
            }
        }

        private static TestResult ConverResultToTestResult(TestCase testCase, Result result, RunStats runStats)
        {
            var testResult = new TestResult(testCase)
            {
                ComputerName = Environment.MachineName,
                Outcome = MapSpecificationResultToTestOutcome(result),
                DisplayName = testCase.DisplayName
            };

            if (result.Exception != null)
            {
                testResult.ErrorMessage = result.Exception.Message;
                testResult.ErrorStackTrace = result.Exception.ToString();
            }

            if (runStats != null)
            {
                testResult.StartTime = runStats.Start;
                testResult.EndTime = runStats.End;
                testResult.Duration = runStats.Duration;
            }

            return testResult;
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnRunStart()
        {
        }

        public void SendErrorMessage(string message, Exception exception)
        {
            frameworkHandle?.SendMessage(TestMessageLevel.Error, message + Environment.NewLine + exception);
        }
    }
}
