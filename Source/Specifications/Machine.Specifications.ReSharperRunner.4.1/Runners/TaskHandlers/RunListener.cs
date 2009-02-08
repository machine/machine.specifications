using System;
using System.Diagnostics;
using System.Threading;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.Runner;

namespace Machine.Specifications.ReSharperRunner.Runners.TaskHandlers
{
  public class RunListener:ISpecificationRunListener
  {
    readonly IRemoteTaskServer _server;
    readonly TaskExecutionNode _node;

    public RunListener(IRemoteTaskServer server, TaskExecutionNode node)
    {
      _server = server;
      _node = node;
    }

    #region Implementation of ISpecificationRunListener
    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      Debug.WriteLine("Assembly start: " + assembly.Name);
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      Debug.WriteLine("Assembly end: " + assembly.Name);
    }

    public void OnRunStart()
    {
      Debug.WriteLine("Run start");
    }

    public void OnRunEnd()
    {
      Debug.WriteLine("Run end");
    }

    public void OnContextStart(ContextInfo context)
    {
      Debug.WriteLine("Context start: " + context.FullName);
    }

    public void OnContextEnd(ContextInfo context)
    {
      Debug.WriteLine("Context end: " + context.FullName);
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      Debug.WriteLine("Specification end: " + specification.Name);
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      Debug.WriteLine(String.Format("Specification end: {0} Result: {1}", specification.Name, result.Status));
    }

    public void OnFatalError(ExceptionResult exception)
    {
      Debug.WriteLine("Fatal error: " + exception.Message);
    }
    #endregion
  }
}