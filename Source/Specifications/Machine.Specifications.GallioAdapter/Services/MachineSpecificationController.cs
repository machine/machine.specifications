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
      MachineSpecificationTest specification = test as MachineSpecificationTest;
      MachineDescriptionTest description = test as MachineDescriptionTest;
      if (specification != null)
      {
        //RunDescriptionTest(specification, testComman);
      }
      else if (description != null)
      {
        RunDescriptionTest(description, testCommand, parentTestStep);
      }
      else
      {
        Debug.WriteLine("Got something weird " + test.GetType().ToString());
      }
    }

    private void RunDescriptionTest(MachineDescriptionTest description, ITestCommand testCommand, ITestStep parentTestStep)
    {
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);

      testContext.LifecyclePhase = LifecyclePhases.SetUp;
      description.SetupContext();
      bool passed = true;

      foreach (ITestCommand child in testCommand.Children)
      {
        MachineSpecificationTest specification = child.Test as MachineSpecificationTest;

        if (specification != null)
        {
          passed &= RunSpecificationTest(specification, child, testContext.TestStep);
        }
      }

      testContext.LifecyclePhase = LifecyclePhases.TearDown;
      description.TeardownContext();

      testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
    }

    private bool RunSpecificationTest(MachineSpecificationTest specification, ITestCommand testCommand, ITestStep parentTestStep)
    {
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
      testContext.LifecyclePhase = LifecyclePhases.Execute;

      var result = specification.Execute();

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