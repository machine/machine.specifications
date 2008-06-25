using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Runner;

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

    public void OnAssemblyStart(Assembly assembly)
    {
    }

    public void OnAssemblyEnd(Assembly assembly)
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

    public void OnContextStart(Model.Context context)
    {
      string line = "";

      if (context.Concern != null)
      {
        line += context.Concern.FullConcern + ", ";
      }

      testListener.WriteLine(line + context.Name, Category.Output);
    }

    public void OnContextEnd(Model.Context context)
    {
      testListener.WriteLine("", Category.Output);
    }

    public void OnSpecificationStart(Specification specification)
    {
    }

    public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
    {
      var formatter = resultFormatterFactory.GetResultFormatterFor(result);
      testListener.WriteLine(formatter.FormatResult(specification), Category.Output);

      TestResult testResult = new TestResult();
      testResult.Name = specification.Name;
      if (result.Passed)
      {
        testResult.State = TestState.Passed;
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
  }
}