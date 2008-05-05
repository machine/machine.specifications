using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using TestDriven.Framework;

namespace Machine.Specifications.TDNetRunner
{
  public class SpecificationRunner : ITestRunner
  {
    public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
    {
      var testResults = new List<TestResult>();
      var explorer = new AssemblyExplorer();
      var specifications = explorer.FindSpecificationsIn(assembly);

      if (specifications.Count() == 0) return TestRunState.NoTests;

      bool failure = false;
      foreach (var specification in specifications)
      {
        testListener.WriteLine(String.Format("{0}\n  When {1}", specification.Name, specification.WhenClause), Category.Output);
        specification.RunContextBeforeAll();

        foreach (var requirement in specification.Requirements)
        {
          var result = specification.VerifyRequirement(requirement);
          var prefix = result.Passed ? "    " : "!!! ";
          var suffix = result.Passed ? "" : " !!!";
          testListener.WriteLine(String.Format("{1}* It {0}{2}", requirement.ItClause, prefix, suffix), Category.Output);

          TestResult testResult = new TestResult();
          testResult.Name = requirement.ItClause;
          if (result.Passed)
          {
            testResult.State = TestState.Passed;
          }
          else
          {
            testResult.State = TestState.Failed;
            testResult.StackTrace = result.Exception.ToString();
            failure = true;
            //testListener.WriteLine(result.Exception.ToString().Split(new [] {'\r', '\n'}).Last(), Category.Output);
          }

          testResults.Add(testResult);
        }

        specification.RunContextAfterAll();
        testListener.WriteLine("", Category.Output);
      }

      foreach (var testResult in testResults)
      {
        testListener.TestFinished(testResult);
      }

      return failure ? TestRunState.Failure : TestRunState.Success;
    }

    public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
    {
      return RunAssembly(testListener, assembly);
    }

    public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
    {
      throw new NotImplementedException();
    }
  }
}
