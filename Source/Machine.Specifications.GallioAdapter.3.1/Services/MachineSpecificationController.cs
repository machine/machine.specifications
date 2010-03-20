// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Modified by and Portions Copyright 2008 Machine Project

using System;
using System.Diagnostics;
using System.Linq;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.Runtime.ProgressMonitoring;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.Utility;

using TestLog = Gallio.Framework.TestLog;

namespace Machine.Specifications.GallioAdapter.Services
{
  public class MachineSpecificationController : TestController
  {            
    protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep,
                    TestExecutionOptions options, IProgressMonitor progressMonitor)
    {
      using (progressMonitor)
      {
        progressMonitor.BeginTask("Verifying Specifications", rootTestCommand.TestCount);

        if (options.SkipTestExecution)
        {
          return SkipAll(rootTestCommand, parentTestStep);
        }
        else
        {
          ITestCommand assemblyCommand = rootTestCommand.Children.SingleOrDefault();
          if( assemblyCommand == null)
            return new TestResult( TestOutcome.Error);

          ITestContext rootContext = rootTestCommand.StartPrimaryChildStep( parentTestStep);

          return RunTest(assemblyCommand, rootContext.TestStep, progressMonitor);                     
        }
      }
    }
     
    TestResult RunTest(ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
    {
      Test test = testCommand.Test;
      progressMonitor.SetStatus(test.Name);

      MachineSpecificationTest specification = test as MachineSpecificationTest;
      MachineContextTest context = test as MachineContextTest;
      MachineAssemblyTest assembly = test as MachineAssemblyTest;
      RootTest root = test as RootTest;      

      if (specification != null)
      {
        return RunSpecificationTest(specification, testCommand, parentTestStep);
      }
      else if (context != null)
      {
        return RunContextTest(context, testCommand, parentTestStep);
      }
      else if (assembly != null)
      {
        return RunAssembly(assembly, testCommand, parentTestStep, progressMonitor);
      }
      else
      {
        Debug.WriteLine("Got something weird " + test.GetType().ToString());
        return new TestResult(TestOutcome.Error);
      }
    }

    TestResult RunAssembly(MachineAssemblyTest assembly, ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
    {
      ITestContext assemblyContext = testCommand.StartPrimaryChildStep(parentTestStep);      

      TestOutcome outcome = TestOutcome.Passed;
      
      assembly.AssemblyContexts.Each(context => context.OnAssemblyStart());
      
      foreach (ITestCommand child in testCommand.Children)
      {
        var childResult = RunTest(child, assemblyContext.TestStep, progressMonitor);
        outcome = outcome.CombineWith(childResult.Outcome);
      }
            
      assembly.AssemblyContexts.Reverse().Each(context => context.OnAssemblyComplete());

      return assemblyContext.FinishStep( outcome, null);
    }

    TestResult RunContextTest(MachineContextTest description, ITestCommand testCommand, TestStep parentTestStep)
    {
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);      
      testContext.LifecyclePhase = LifecyclePhases.SetUp;
      description.SetupContext();

      TestOutcome outcome = TestOutcome.Passed;

      foreach (ITestCommand child in testCommand.Children)
      {
        MachineSpecificationTest specification = child.Test as MachineSpecificationTest;

        if (specification != null)
        {
          var childResult = RunSpecificationTest(specification, child, testContext.TestStep);
          outcome  = outcome.CombineWith( childResult.Outcome);
        }
      }

      testContext.LifecyclePhase = LifecyclePhases.TearDown;
      description.TeardownContext();

      return testContext.FinishStep(outcome, null);
    }

    TestResult RunSpecificationTest(MachineSpecificationTest specification, ITestCommand testCommand, TestStep parentTestStep)
    {      
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
      testContext.LifecyclePhase = LifecyclePhases.Execute;

      var result = specification.Execute();           

      if (result.Status == Status.NotImplemented)
      {        
        TestLog.Warnings.WriteLine("{0} ({1})", specification.Name, "NOT IMPLEMENTED");
        TestLog.Warnings.Flush();

        return testContext.FinishStep(TestOutcome.Pending, new TimeSpan(0));
      }
      else if (result.Status == Status.Ignored)
      {
        TestLog.Warnings.WriteLine("{0} ({1})", specification.Name, "IGNORED");
        TestLog.Warnings.Flush();
        
        return testContext.FinishStep(TestOutcome.Ignored, new TimeSpan(0));
      }        
      else if (result.Passed)
      {
        return testContext.FinishStep(TestOutcome.Passed, null);
      }
      else
      {        
        var ex = result.Exception;
        var data = new Gallio.Common.Diagnostics.ExceptionData(ex.TypeName, ex.Message, ex.StackTrace, null);

        TestLog.Failures.WriteException(data);
        TestLog.Failures.Flush();
        
        return testContext.FinishStep(TestOutcome.Failed, null);
      }      
    }
  }
}