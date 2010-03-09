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
using Gallio.Runtime.ProgressMonitoring;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.Utility;
using Gallio.Model.Helpers;
using Gallio.Model.Commands;
using Gallio.Model.Tree;
using Gallio.Model.Contexts;

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
       
        private TestResult RunTest(ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            Test test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);

            MachineSpecificationTest specification = test as MachineSpecificationTest;
            MachineContextTest context = test as MachineContextTest;
            MachineAssembly assembly = test as MachineAssembly;
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

        private TestResult RunAssembly(MachineAssembly assembly, ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext assemblyContext = testCommand.StartPrimaryChildStep(parentTestStep);
            
            bool passed = true;
            
            // Setup
            assembly.Contexts.Each(context => context.OnAssemblyStart());
            
            foreach (ITestCommand child in testCommand.Children)
            {
                var childResult = RunTest(child, assemblyContext.TestStep, progressMonitor);
                passed &= childResult.Outcome.Status == TestStatus.Passed;

            }
            
            // Take down
            assembly.Contexts.Each(context => context.OnAssemblyComplete());

            return new TestResult(passed ? TestOutcome.Passed : TestOutcome.Failed);
        }

        private TestResult RunContextTest(MachineContextTest description, ITestCommand testCommand, TestStep parentTestStep)
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
                    var childResult = RunSpecificationTest(specification, child, testContext.TestStep);
                    passed &= childResult.Outcome.Status == TestStatus.Passed;                                       
                }
            }

            testContext.LifecyclePhase = LifecyclePhases.TearDown;
            description.TeardownContext();

            return testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
        }

        private TestResult RunSpecificationTest(MachineSpecificationTest specification, ITestCommand testCommand, TestStep parentTestStep)
        {            
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            testContext.LifecyclePhase = LifecyclePhases.Execute;

            var result = specification.Execute();
            
            // Get other failed states here

            if (result.Passed)
            {
                return testContext.FinishStep(TestOutcome.Passed, null);
            }
            else
            {
                return testContext.FinishStep(TestOutcome.Failed, null);
            }            
        }
    }
}