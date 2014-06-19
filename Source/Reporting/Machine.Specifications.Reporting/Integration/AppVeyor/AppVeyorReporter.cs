namespace Machine.Specifications.Reporting.Integration.AppVeyor
{
    using System;
    using Runner.Utility;

    public class AppVeyorReporter : SpecificationRunListenerBase
    {
        const string FrameworkName = "Machine.Specifications";

        readonly Action<string> _writer;
        readonly IAppVeyorBuildWorkerApiClient _apiClient;
        readonly TimingRunListener _timingListener;

        AssemblyInfo _currentAssembly;
        string _currentContext;
        bool _failureOccurred;

        public AppVeyorReporter(Action<string> writer, IAppVeyorBuildWorkerApiClient apiClient, TimingRunListener listener)
        {
            _writer = writer;
            _apiClient = apiClient;
            _timingListener = listener;
            _failureOccurred = false;
        }

        public bool FailureOccurred
        {
            get { return _failureOccurred; }
        }

        protected override void OnAssemblyStart(AssemblyInfo assembly)
        {
            _writer(string.Format("Assembly: {0}", assembly.Name));
            _currentAssembly = assembly;
        }

        protected override void OnContextStart(ContextInfo context)
        {
            _writer(string.Format("  Context: {0}", context.Name));
            _currentContext = context.FullName;
        }

        protected override void OnContextEnd(ContextInfo context)
        {
            _currentContext = "";
        }

        protected override void OnSpecificationStart(SpecificationInfo specificationInfo)
        {            
            _apiClient.AddTest(specificationInfo.Name,
                               FrameworkName,
                               _currentAssembly.Name,
                               "Running",
                               null,
                               null,
                               null,
                               null,
                               null);
            _writer(string.Format("    Specification: {0}", specificationInfo.Name));
        }

        protected override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            TimeSpan duration = TimeSpan.FromMilliseconds(_timingListener.GetSpecificationTime(specification));

            switch (result.Status)
            {
                case Status.Passing:
                    UpdateTestPassed(specification, duration);
                    break;
                case Status.NotImplemented:
                    UpdateTestSkipped(specification, duration);
                    break;
                case Status.Ignored:
                    UpdateTestSkipped(specification, duration);
                    break;
                default:
                    UpdateTestFailed(specification, duration, result.Exception);
                    _writer(string.Format("    ^^^^ FAILURE {0}", specification.Name));
                    _failureOccurred = true;
                    break;
            }
        }

        void UpdateTestPassed(SpecificationInfo specification, TimeSpan executionTime)
        {
            _apiClient.UpdateTest(specification.Name,
                                  FrameworkName,
                                  _currentAssembly.Name,
                                  "Passed",
                                  Convert.ToInt64(executionTime.TotalMilliseconds),
                                  null,
                                  null,
                                  specification.CapturedOutput,
                                  null);
        }

        void UpdateTestFailed(SpecificationInfo specification,
                              TimeSpan executionTime,
                              ExceptionResult exceptionResult = null)
        {
            string errorMessage = null;
            string errorStackTrace = null;
            if (exceptionResult != null)
            {
                errorMessage = exceptionResult.Message;
                errorStackTrace = exceptionResult.ToString();
            }

            _apiClient.UpdateTest(specification.Name,
                                  FrameworkName,
                                  _currentAssembly.Name,
                                  "Failed",
                                  Convert.ToInt64(executionTime.TotalMilliseconds),
                                  errorMessage,
                                  errorStackTrace,
                                  specification.CapturedOutput,
                                  null);
        }

        void UpdateTestSkipped(SpecificationInfo specification, TimeSpan executionTime)
        {
            _apiClient.UpdateTest(specification.Name,
                                  FrameworkName,
                                  _currentAssembly.Name,
                                  "Skipped",
                                  Convert.ToInt64(executionTime.TotalMilliseconds),
                                  null,
                                  null,
                                  null,
                                  null);
        }

        protected override void OnFatalError(ExceptionResult exception)
        {
            _failureOccurred = true;
        }

        protected string GetSpecificationName(SpecificationInfo specification)
        {
            return _currentContext + " > " + specification.Name;
        }
    }
}