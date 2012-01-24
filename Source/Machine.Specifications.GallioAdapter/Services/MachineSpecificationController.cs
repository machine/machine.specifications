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
          TestOutcome outcome = TestOutcome.Passed;
                              
          _progressMonitor = progressMonitor;
          SetupRunOptions(options);
          SetupListeners(options);

          _listener.OnRunStart();

          foreach (ITestCommand command in rootTestCommand.Children)
          {
            MachineAssemblyTest assemblyTest = command.Test as MachineAssemblyTest;
            if( assemblyTest == null )
              continue;                        

            var assemblyResult = RunAssembly(assemblyTest, command, rootStep);
            outcome = outcome.CombineWith( assemblyResult.Outcome);
          }

          _listener.OnRunEnd();

          return rootContext.FinishStep( outcome, null);
        }
      }      
    }      

    void SetupRunOptions(TestExecutionOptions options)
    {
      // Gallio has an extremely flexible mechanism for defining test filters
      // For now I will just try to pick out tags that should be included or excluded
      var metaFilters = from filter in options.FilterSet.Rules
                        let rule = filter.Filter as Gallio.Model.Filters.MetadataFilter<Gallio.Model.Filters.ITestDescriptor>
                        where rule != null
                        select new { RuleType = filter.RuleType, Rule = rule };
      var tagFilters = from meta in metaFilters
                       let rule = meta.Rule
                       let value = rule.ValueFilter as Gallio.Model.Filters.EqualityFilter<string>
                       where value != null && rule.Key == "Tag"
                       group value.Comparand by meta.RuleType into g
                       select g;

      var includeTags = tagFilters.SingleOrDefault(g => g.Key == Gallio.Model.Filters.FilterRuleType.Inclusion) ?? Enumerable.Empty<string>();
      var excludeTags = tagFilters.SingleOrDefault(g => g.Key == Gallio.Model.Filters.FilterRuleType.Exclusion) ?? Enumerable.Empty<string>();

      _options = new RunOptions(includeTags, excludeTags, new string[0]);
    }

    void SetupListeners(TestExecutionOptions options)
    {
      _listener = new AggregateRunListener(Enumerable.Empty<ISpecificationRunListener>());
    }    

    TestResult RunAssembly(MachineAssemblyTest assemblyTest, ITestCommand command, TestStep parentTestStep)
    {
      ITestContext assemblyContext = command.StartPrimaryChildStep(parentTestStep);

      AssemblyInfo assemblyInfo = new AssemblyInfo(assemblyTest.Name, assemblyTest.AssemblyFilePath);
      TestOutcome outcome = TestOutcome.Passed;

      _listener.OnAssemblyStart(assemblyInfo);
      assemblyTest.AssemblyContexts.Each(context => context.OnAssemblyStart());
      
      foreach (ITestCommand contextCommand in command.Children)
      {
        MachineContextTest contextTest = contextCommand.Test as MachineContextTest;
        if (contextTest == null) 
          continue;

        var contextResult = RunContextTest( assemblyTest, contextTest, contextCommand, assemblyContext.TestStep);
        outcome = outcome.CombineWith(contextResult.Outcome);
        assemblyContext.SetInterimOutcome(outcome);
      }
            
      assemblyTest.AssemblyContexts.Reverse().Each(context => context.OnAssemblyComplete());
      _listener.OnAssemblyEnd(assemblyInfo);

      return assemblyContext.FinishStep( outcome, null);
    }

    TestResult RunContextTest(MachineAssemblyTest assemblyTest, MachineContextTest contextTest, ITestCommand command, TestStep parentTestStep)
    {
      ITestContext testContext = command.StartPrimaryChildStep(parentTestStep);

      GallioRunListener listener = new GallioRunListener(_listener, _progressMonitor, testContext, command.Children);

      IContextRunner runner = ContextRunnerFactory.GetContextRunnerFor(contextTest.Context);                    
      runner.Run(contextTest.Context, listener, _options, assemblyTest.GlobalCleanup, assemblyTest.SpecificationSupplements);

      return testContext.FinishStep(listener.Outcome, null);
    }
  }
}