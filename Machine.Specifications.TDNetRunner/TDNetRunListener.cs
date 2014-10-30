using System.Collections.Generic;
using Machine.Specifications.Runner.Utility;
using TestDriven.Framework;

namespace Machine.Specifications.TDNetRunner
{
    public class TDNetRunListener : ISpecificationRunListener
    {
        readonly ITestListener testListener;
        readonly ResultFormatterFactory resultFormatterFactory;
        TestRunState testRunState = TestRunState.NoTests;
        readonly List<TestResult> testResults = new List<TestResult>();

        public TestRunState TestRunState
        {
            get { return testRunState; }
        }

        public TDNetRunListener(ITestListener testListener)
        {
            this.testListener = testListener;
            resultFormatterFactory = new ResultFormatterFactory();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
            if (testResults.Count == 0) return;

            bool failure = false;

            foreach (var testResult in testResults)
            {
                testListener.TestFinished(testResult);
                failure |= testResult.State == TestState.Failed;
            }

            testRunState = failure ? TestRunState.Failure : TestRunState.Success;
        }

        public void OnContextStart(ContextInfo context)
        {
            testListener.WriteLine(context.FullName, Category.Output);
        }

        public void OnContextEnd(ContextInfo context)
        {
            testListener.WriteLine("", Category.Output);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            var formatter = resultFormatterFactory.GetResultFormatterFor(result);
            testListener.WriteLine(formatter.FormatResult(specification), Category.Output);

            TestResult testResult = new TestResult();
            testResult.Name = specification.Name;
            if (result.Passed)
            {
                testResult.State = TestState.Passed;
            }
            else if (result.Status == Status.Ignored)
            {
                testResult.State = TestState.Ignored;
                testResult.Message = "Ignored";
            }
            else if (result.Status == Status.NotImplemented)
            {
                testResult.State = TestState.Ignored;
                testResult.Message = "Not Implemented";
            }
            else
            {
                testResult.State = TestState.Failed;
                if (result.Exception != null)
                {
                    testResult.StackTrace = result.Exception.ToString();
                }
            }

            testResults.Add(testResult);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            testListener.WriteLine("Fatal error: " + exception.ToString(), Category.Error);
        }
    }
}