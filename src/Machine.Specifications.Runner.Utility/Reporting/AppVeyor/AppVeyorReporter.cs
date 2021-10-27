using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Integration.AppVeyor
{
    using System;
    using System.Diagnostics;

    public class AppVeyorReporter //: ISpecificationRunListener, ISpecificationResultProvider
    {
        const string FrameworkName = "Machine.Specifications";

        readonly Action<string> _writer;
        readonly IAppVeyorBuildWorkerApiClient _apiClient;
        readonly Stopwatch _specificationTimer = new Stopwatch();

        AssemblyInfo _currentAssembly;
        string _currentContext;
        bool _failureOccurred;

        public AppVeyorReporter(Action<string> writer, IAppVeyorBuildWorkerApiClient apiClient)
        {
            _writer = writer;
            _apiClient = apiClient;
            _failureOccurred = false;
        }

        public bool FailureOccurred
        {
            get { return _failureOccurred; }
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
            _writer("");
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            _writer(string.Format("\nSpecs in {0}:", assembly.Name));
            _currentAssembly = assembly;
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnContextStart(ContextInfo context)
        {
            _writer(string.Format("\n{0}", context.FullName));
            _currentContext = context.FullName;
        }

        public void OnContextEnd(ContextInfo context)
        {
            _currentContext = "";
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            _specificationTimer.Reset();
            _specificationTimer.Start();

            _apiClient.AddTest(GetSpecificationName(specification),
                               FrameworkName,
                               _currentAssembly.Name,
                               "Running",
                               null,
                               null,
                               null,
                               null,
                               null);
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _specificationTimer.Stop();
            long duration = _specificationTimer.ElapsedMilliseconds;

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
                    _failureOccurred = true;
                    break;
            }
        }

        void UpdateTestPassed(SpecificationInfo specification, long executionTime)
        {
            _writer(string.Format("\x1B[32m* {0}\x1B[39m", specification.Name)); // green
            _apiClient.UpdateTest(GetSpecificationName(specification),
                                  FrameworkName,
                                  _currentAssembly.Name,
                                  "Passed",
                                  executionTime,
                                  null,
                                  null,
                                  specification.CapturedOutput,
                                  null);
        }

        void UpdateTestFailed(SpecificationInfo specification, long executionTime, ExceptionResult exceptionResult = null)
        {
            _writer(string.Format("\x1B[31m* {0} (FAILED)\x1B[39m", specification.Name)); // red

            string errorMessage = null;
            string errorStackTrace = null;
            if (exceptionResult != null)
            {
                errorMessage = exceptionResult.Message;
                errorStackTrace = exceptionResult.ToString();
            }

            _apiClient.UpdateTest(GetSpecificationName(specification),
                                  FrameworkName,
                                  _currentAssembly.Name,
                                  "Failed",
                                  executionTime,
                                  errorMessage,
                                  errorStackTrace,
                                  specification.CapturedOutput,
                                  null);
        }

        void UpdateTestIgnored(SpecificationInfo specification, long executionTime)
        {
            _writer(string.Format("\x1B[33m* {0} (IGNORED)\x1B[39m", specification.Name)); // yellow
            _apiClient.UpdateTest(GetSpecificationName(specification),
                                  FrameworkName,
                                  _currentAssembly.Name,
                                  "Skipped",
                                  executionTime,
                                  null,
                                  null,
                                  null,
                                  null);
        }

        void UpdateTestNotImplemented(SpecificationInfo specification, long executionTime)
        {
            _writer(string.Format("\x1B[90m* {0} (NOT IMPLEMENTED)\x1B[39m", specification.Name)); // gray
            _apiClient.UpdateTest(GetSpecificationName(specification),
                                  FrameworkName,
                                  _currentAssembly.Name,
                                  "NotImplemented",
                                  executionTime,
                                  null,
                                  null,
                                  null,
                                  null);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            _failureOccurred = true;
        }

        protected string GetSpecificationName(SpecificationInfo specification)
        {
            return _currentContext + " » " + specification.Name;
        }
    }
}
