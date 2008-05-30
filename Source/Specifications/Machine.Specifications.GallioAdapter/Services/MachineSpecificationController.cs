// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Modified by and Portions Copyright 2008 Machine Project

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