using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.TDNetRunner;
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
      testListener.WriteLine(context.Name, Category.Output);
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

  public class SpecificationRunner : ITestRunner
  {
    public SpecificationRunner()
    {
    }

    public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new Runner.SpecificationRunner(listener);
      runner.RunAssembly(assembly);

      return listener.TestRunState;
    }

    public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new Runner.SpecificationRunner(listener);
      runner.RunNamespace(assembly, ns);

      return listener.TestRunState;
    }

    public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
    {
      var listener = new TDNetRunListener(testListener);
      var runner = new Runner.SpecificationRunner(listener);
      runner.RunMember(assembly, member);

      return listener.TestRunState;
    }
  }
}
