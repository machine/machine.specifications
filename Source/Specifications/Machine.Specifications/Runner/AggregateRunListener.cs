using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  public class AggregateRunListener : ISpecificationRunListener
  {
    readonly List<ISpecificationRunListener> _listeners;

    public AggregateRunListener(IEnumerable<ISpecificationRunListener> listeners)
    {
      _listeners = new List<ISpecificationRunListener>(listeners);
    }

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      _listeners.ForEach(listener => listener.OnAssemblyStart(assembly));
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      _listeners.ForEach(listener => listener.OnAssemblyEnd(assembly));
    }

    public void OnRunStart()
    {
      _listeners.ForEach(listener => listener.OnRunStart());
    }

    public void OnRunEnd()
    {
      _listeners.ForEach(listener => listener.OnRunEnd());
    }

    public void OnContextStart(ContextInfo context)
    {
      _listeners.ForEach(listener => listener.OnContextStart(context));
    }

    public void OnContextEnd(ContextInfo context)
    {
      _listeners.ForEach(listener => listener.OnContextEnd(context));
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      _listeners.ForEach(listener => listener.OnSpecificationStart(specification));
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Machine.Specifications.Model.SpecificationVerificationResult result)
    {
      _listeners.ForEach(listener => listener.OnSpecificationEnd(specification,result));

    }
  }
}
