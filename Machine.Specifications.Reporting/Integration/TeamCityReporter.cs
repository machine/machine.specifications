using System;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Integration
{
    public class TeamCityReporter : SpecificationRunListenerBase
    {
        readonly TimingRunListener _timingListener;

        readonly TeamCityServiceMessageWriter _writer;
        string _currentContext;
        string _currentNamespace;
        bool _failureOccurred;

        public TeamCityReporter(Action<string> writer, TimingRunListener listener)
        {
            _timingListener = listener;
            _failureOccurred = false;
            _writer = new TeamCityServiceMessageWriter(writer);
        }

        public bool FailureOccurred
        {
            get { return _failureOccurred; }
        }

        protected override void OnAssemblyStart(AssemblyInfo assembly)
        {
            _writer.WriteProgressStart("Running specifications in " + assembly.Name);
        }

        protected override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            if (!string.IsNullOrEmpty(_currentNamespace))
            {
                _writer.WriteTestSuiteFinished(_currentNamespace);
            }
            _writer.WriteProgressFinish("Running specifications in " + assembly.Name);
        }

        protected override void OnRunStart()
        {
            _writer.WriteProgressStart("Running specifications.");
        }

        protected override void OnRunEnd()
        {
            _writer.WriteProgressFinish("Running specifications.");
        }

        protected override void OnContextStart(ContextInfo context)
        {
            if (context.Namespace != _currentNamespace)
            {
                if (!string.IsNullOrEmpty(_currentNamespace))
                {
                    _writer.WriteTestSuiteFinished(_currentNamespace);
                }
                _currentNamespace = context.Namespace;
                _writer.WriteTestSuiteStarted(_currentNamespace);
            }
            _currentContext = context.FullName;
        }

        protected override void OnContextEnd(ContextInfo context)
        {
            _currentContext = "";
        }

        protected override void OnSpecificationStart(SpecificationInfo specification)
        {
            _writer.WriteTestStarted(GetSpecificationName(specification), false);
        }

        protected override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            switch (result.Status)
            {
                case Status.Passing:
                    break;
                case Status.NotImplemented:
                    _writer.WriteTestIgnored(GetSpecificationName(specification), "(Not Implemented)");
                    break;
                case Status.Ignored:
                    _writer.WriteTestIgnored(GetSpecificationName(specification), "(Ignored)");
                    break;
                default:
                    if (result.Exception != null)
                    {
                        _writer.WriteTestFailed(GetSpecificationName(specification),
                                                result.Exception.Message,
                                                result.Exception.ToString());
                    }
                    else
                    {
                        _writer.WriteTestFailed(GetSpecificationName(specification), "FAIL", "");
                    }
                    _failureOccurred = true;
                    break;
            }
            var duration = TimeSpan.FromMilliseconds(_timingListener.GetSpecificationTime(specification));

            _writer.WriteTestFinished(GetSpecificationName(specification), duration);
        }

        protected override void OnFatalError(ExceptionResult exception)
        {
            _writer.WriteError(exception.Message, exception.ToString());
            _failureOccurred = true;
        }

        protected string GetSpecificationName(SpecificationInfo specification)
        {
            return _currentContext + " > " + specification.Name;
        }
    }
}