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
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.GallioAdapter.Services
{  
  public class MachineSpecificationController : TestController
  {
    ISpecificationRunListener _listener;
    RunOptions _options;
    IProgressMonitor _progressMonitor;

    protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep,
                    TestExecutionOptions options, IProgressMonitor progressMonitor)
    {
      _listener = new AggregateRunListener(Enumerable.Empty<ISpecificationRunListener>());
      _options = new RunOptions(Enumerable.Empty<string>(), Enumerable.Empty<string>());
      _progressMonitor = progressMonitor;

      using (progressMonitor)
      {
        progressMonitor.BeginTask("Verifying Specifications", rootTestCommand.TestCount);

        if (options.SkipTestExecution)
        {
          return SkipAll(rootTestCommand, parentTestStep);
        }
        else
        {
          ITestContext rootContext = rootTestCommand.StartPrimaryChildStep(parentTestStep);
          TestStep rootStep = rootContext.TestStep;          
          TestOutcome outcome = TestOutcome.Pending;

          _listener.OnRunStart();

          foreach (ITestCommand command in rootTestCommand.Children)
          {
            MachineAssemblyTest assembly = command.Test as MachineAssemblyTest;
            if( assembly == null )
              continue;                        

            var assemblyResult = RunAssembly(assembly, command, rootStep);
            outcome = outcome.CombineWith( assemblyResult.Outcome);
          }

          _listener.OnRunEnd();

          return rootContext.FinishStep( outcome, null);
        }
      }      
    }      

    TestResult RunAssembly(MachineAssemblyTest assembly, ITestCommand testCommand, TestStep parentTestStep)
    {
      ITestContext assemblyContext = testCommand.StartPrimaryChildStep(parentTestStep);      

      AssemblyInfo assemblyInfo = new AssemblyInfo( assembly.Name);
      TestOutcome outcome = TestOutcome.Passed;

      _listener.OnAssemblyStart(assemblyInfo);
      assembly.AssemblyContexts.Each(context => context.OnAssemblyStart());
      
      foreach (ITestCommand contextCommand in testCommand.Children)
      {
        MachineContextTest contextTest = contextCommand.Test as MachineContextTest;
        if (contextTest == null) 
          continue;

        var childResult = RunContextTest( contextTest, contextCommand, assemblyContext.TestStep);
        outcome = outcome.CombineWith(childResult.Outcome);
        assemblyContext.SetInterimOutcome(outcome);
      }
            
      assembly.AssemblyContexts.Reverse().Each(context => context.OnAssemblyComplete());
      _listener.OnAssemblyEnd(assemblyInfo);

      return assemblyContext.FinishStep( outcome, null);
    }

    TestResult RunContextTest(MachineContextTest contextTest, ITestCommand testCommand, TestStep parentTestStep)
    {
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);      
      testContext.LifecyclePhase = LifecyclePhases.SetUp;

      IContextRunner runner = ContextRunnerFactory.GetContextRunnerFor(contextTest.Context);      

      GallioRunListener listener = new GallioRunListener( _listener, _progressMonitor, testContext, testCommand.Children);
        
      runner.Run(contextTest.Context, listener, _options, 
        Enumerable.Empty<ICleanupAfterEveryContextInAssembly>(), 
        Enumerable.Empty<ISupplementSpecificationResults>());

      return testContext.FinishStep(listener.Outcome, null);
    }

    TestResult RunSpecificationTest(MachineSpecificationTest specification, ITestCommand testCommand, TestStep parentTestStep)
    {      
      ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
      testContext.LifecyclePhase = LifecyclePhases.Execute;

      var result = Result.Pass();// specification.Execute();           

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