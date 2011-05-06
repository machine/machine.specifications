using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gallio.Common.Collections;

using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.GallioAdapter.Model;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Framework;
using Gallio.Model.Contexts;

namespace Machine.Specifications.GallioAdapter.Services
{
  public class GallioRunListener : RunListenerBase
  {
    ISpecificationRunListener _listener;
    IProgressMonitor _progressMonitor;
    ITestContext _testContext;

    Dictionary<string, ITestCommand> _commandsBySpec;
    Dictionary<string, ITestContext> _contextsBySpec;

    TestOutcome _outcome;
    public TestOutcome Outcome { get { return _outcome; } }

    public GallioRunListener(ISpecificationRunListener listener, IProgressMonitor progressMonitor, 
      ITestContext context, IEnumerable<ITestCommand> specificationCommands)     
    {
      _listener = listener;
      _progressMonitor = progressMonitor;
      _testContext = context;

      _outcome = TestOutcome.Passed;

      _contextsBySpec = new Dictionary<string, ITestContext>();      
      _commandsBySpec = (from command in specificationCommands
                         let specificationTest = command.Test as MachineSpecificationTest
                         where specificationTest != null
                         select new { Name = specificationTest.Specification.Name, Command = command })
                         .ToDictionary(x => x.Name, x => x.Command);
    }

    public override void OnContextStart(ContextInfo context)
    {
      _listener.OnContextStart(context);
      _testContext.LifecyclePhase = LifecyclePhases.Starting;
      _progressMonitor.SetStatus(context.FullName);   
    }

    public override void OnContextEnd(ContextInfo context)
    {
      _listener.OnContextEnd(context);
      _testContext.LifecyclePhase = LifecyclePhases.Finishing;      
    }

    public override void OnSpecificationStart(SpecificationInfo specification)
    {
      _listener.OnSpecificationStart(specification);

      ITestCommand specCommand = _commandsBySpec[specification.Name];
      ITestContext specContext =  specCommand.StartPrimaryChildStep(_testContext.TestStep);
      _contextsBySpec.Add(specification.Name, specContext);

      specContext.LifecyclePhase = LifecyclePhases.Starting;
      _progressMonitor.SetStatus("» " + specification.Name);       
    }

    public override void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      _listener.OnSpecificationEnd(specification, result);

      ITestContext testContext = _contextsBySpec[specification.Name];
      testContext.LifecyclePhase = LifecyclePhases.Finishing;

      TestOutcome outcome;
      TimeSpan? span = null;

      if (result.Status == Status.NotImplemented)
      {
        TestLog.Warnings.WriteLine("{0} ({1})", specification.Name, "NOT IMPLEMENTED");
        TestLog.Warnings.Flush();

        outcome = TestOutcome.Pending;
        span = new TimeSpan(0);
      }
      else if (result.Status == Status.Ignored)
      {
        TestLog.Warnings.WriteLine("{0} ({1})", specification.Name, "IGNORED");
        TestLog.Warnings.Flush();

        outcome = TestOutcome.Ignored;
        span = new TimeSpan(0);        
      }
      else if (result.Passed)
      {
        outcome = TestOutcome.Passed;
      }
      else
      {        
        TestLog.Failures.WriteException( Convert( result.Exception));
        TestLog.Failures.Flush();

        outcome = TestOutcome.Failed;
      }
       
      testContext.FinishStep(outcome, span);
      _outcome = _outcome.CombineWith(outcome);

      _progressMonitor.Worked(1);
    }

    public override void OnFatalError(ExceptionResult exception)
    {
      _listener.OnFatalError(exception);
    }

    Gallio.Common.Diagnostics.ExceptionData Convert(ExceptionResult result)
    {
      if (result == null) 
        return null;

      Gallio.Common.Diagnostics.ExceptionData inner = Convert(result.InnerExceptionResult);
      return new Gallio.Common.Diagnostics.ExceptionData(result.TypeName, result.Message, result.StackTrace, new PropertySet(), inner);
    }
  }
}
