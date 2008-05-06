using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private AssemblyExplorer explorer;
    public SpecificationRunner()
    {
      explorer = new AssemblyExplorer();
    }

    public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
    {
      var descriptions = explorer.FindDescriptionsIn(assembly);

      return RunDescriptions(descriptions, testListener);
    }

    private TestRunState RunDescriptions(IEnumerable<Description> descriptions, ITestListener testListener)
    {
      if (descriptions.Count() == 0) return TestRunState.NoTests;

      var testResults = new List<TestResult>();
      bool failure = false;

      foreach (var description in descriptions)
      {
        testListener.WriteLine(String.Format("{0}\n  When {1}", description.Name, description.WhenClause), Category.Output);
        description.RunContextBeforeAll();

        foreach (var specification in description.Specifications)
        {
          var result = description.VerifySpecification(specification);
          var prefix = result.Passed ? "    " : "!!! ";
          var suffix = result.Passed ? "" : " !!!";
          testListener.WriteLine(String.Format("{1}* It {0}{2}", specification.ItClause, prefix, suffix), Category.Output);

          TestResult testResult = new TestResult();
          testResult.Name = specification.ItClause;
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

        description.RunContextAfterAll();
        testListener.WriteLine("", Category.Output);
      }

      foreach (var testResult in testResults)
      {
        testListener.TestFinished(testResult);
      }

      return failure ? TestRunState.Failure : TestRunState.Success;
    }

    public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string targetNamespace)
    {
      var descriptions = explorer.FindDescriptionsIn(assembly, targetNamespace);

      return RunDescriptions(descriptions, testListener);
    }

    public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
    {
      if (member.MemberType != MemberTypes.TypeInfo)
      {
        return TestRunState.NoTests;
      }

      Type type = (Type)member;
      var description = explorer.FindDescription(type);

      return RunDescriptions(new[] {description}, testListener);
    }
  }
}
