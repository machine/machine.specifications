using System;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Integration.TeamCity
{
    public class TeamCityReporter
    {
        private readonly TimingRunListener timingListener;

        private readonly TeamCityServiceMessageWriter writer;

        private string currentContext;

        private string currentNamespace;

        private bool failureOccurred;

        public TeamCityReporter(Action<string> writer, TimingRunListener listener)
        {
            this.writer = new TeamCityServiceMessageWriter(writer);

            timingListener = listener;
            failureOccurred = false;
        }

        public bool FailureOccurred => failureOccurred;

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            writer.WriteProgressStart("Running specifications in " + assembly.Name);
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            if (!string.IsNullOrEmpty(currentNamespace))
            {
                writer.WriteTestSuiteFinished(currentNamespace);
            }

            writer.WriteProgressFinish("Running specifications in " + assembly.Name);
        }

        public void OnRunStart()
        {
            writer.WriteProgressStart("Running specifications.");
        }

        public void OnRunEnd()
        {
            writer.WriteProgressFinish("Running specifications.");
        }

        public void OnContextStart(ContextInfo context)
        {
            if (context.Namespace != currentNamespace)
            {
                if (!string.IsNullOrEmpty(currentNamespace))
                {
                    writer.WriteTestSuiteFinished(currentNamespace);
                }

                currentNamespace = context.Namespace;
                writer.WriteTestSuiteStarted(currentNamespace);
            }

            currentContext = context.FullName;
        }

        public void OnContextEnd(ContextInfo context)
        {
            currentContext = string.Empty;
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            writer.WriteTestStarted(GetSpecificationName(specification), false);
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            switch (result.Status)
            {
                case Status.Passing:
                    break;

                case Status.NotImplemented:
                    writer.WriteTestIgnored(GetSpecificationName(specification), "(Not Implemented)");
                    break;

                case Status.Ignored:
                    writer.WriteTestIgnored(GetSpecificationName(specification), "(Ignored)");
                    break;

                default:
                    if (result.Exception != null)
                    {
                        writer.WriteTestFailed(GetSpecificationName(specification),
                                                result.Exception.Message,
                                                result.Exception.ToString());
                    }
                    else
                    {
                        writer.WriteTestFailed(GetSpecificationName(specification), "FAIL", "");
                    }
                    failureOccurred = true;
                    break;
            }

            var duration = TimeSpan.FromMilliseconds(timingListener.GetSpecificationTime(specification));

            writer.WriteTestFinished(GetSpecificationName(specification), duration);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            writer.WriteError(exception.Message, exception.ToString());
            failureOccurred = true;
        }

        protected string GetSpecificationName(SpecificationInfo specification)
        {
            return currentContext + " > " + specification.Name;
        }
    }
}
