using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runtime.ProgressMonitoring;
using Machine.Specifications.GallioAdapter.Model;

namespace Machine.Specifications.GallioAdapter.Services
{
  public class MachineSpecificationController : BaseTestController
  {
    protected override void RunTestsInternal(ITestCommand rootTestCommand, ITestStep parentTestStep,
                                             TestExecutionOptions options, IProgressMonitor progressMonitor)
    {
      using (progressMonitor)
      {
        progressMonitor.BeginTask("Verifying Specifications", rootTestCommand.TestCount);

        if (options.SkipTestExecution)
        {
          SkipAll(rootTestCommand, parentTestStep);
        }
        else
        {
          RunTest(rootTestCommand, parentTestStep, progressMonitor);
        }
      }
    }

    private void RunTest(ITestCommand testCommand, ITestStep parentTestStep, IProgressMonitor progressMonitor)
    {
      ITest test = testCommand.Test;
      progressMonitor.SetStatus(test.Name);

      bool passed;
      MachineRequirementTest requirement = test as MachineRequirementTest;
      MachineSpecificationTest specification = test as MachineSpecificationTest;
      if (requirement != null)
      {
        //RunRequirement(requirement, testComman);
      }
      else if (specification != null)
      {
        RunSpecification(specification, testCommand, parentTestStep);
      }
      else
      {
        Debug.WriteLine("Got something weird " + test.GetType().ToString());
      }
    }

    private void RunSpecification(MachineSpecificationTest specification, ITestCommand testCommand, ITestStep parentTestStep)
    {
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);

      testContext.LifecyclePhase = LifecyclePhases.SetUp;
      specification.SetupContext();
      bool passed = true;

      foreach (ITestCommand child in testCommand.Children)
      {
        MachineRequirementTest requirement = child.Test as MachineRequirementTest;

        if (requirement != null)
        {
          passed &= RunRequirement(requirement, child, testContext.TestStep);
        }
      }

      testContext.LifecyclePhase = LifecyclePhases.TearDown;
      specification.TeardownContext();

      testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
    }

    private bool RunRequirement(MachineRequirementTest requirement, ITestCommand testCommand, ITestStep parentTestStep)
    {
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
      testContext.LifecyclePhase = LifecyclePhases.Execute;

      var result = requirement.Execute();

      if (result.Passed)
      {
        testContext.FinishStep(TestOutcome.Passed, null);
      }
      else
      {
        testContext.FinishStep(TestOutcome.Failed, null);
      }

      return result.Passed;
    }
  }
}