using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  public class AggregateRunListener : ISpecificationRunListener
  {
    List<ISpecificationRunListener> _listeners;

    public AggregateRunListener(IEnumerable<ISpecificationRunListener> listeners)
    {
      _listeners = new List<ISpecificationRunListener>(listeners);
    }

    public void OnAssemblyStart(System.Reflection.Assembly assembly)
    {
      _listeners.ForEach(listener => listener.OnAssemblyStart(assembly));
    }

    public void OnAssemblyEnd(System.Reflection.Assembly assembly)
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

    public void OnContextStart(Machine.Specifications.Model.Context context)
    {
      _listeners.ForEach(listener => listener.OnContextStart(context));
    }

    public void OnContextEnd(Machine.Specifications.Model.Context context)
    {
      _listeners.ForEach(listener => listener.OnContextEnd(context));
    }

    public void OnSpecificationStart(Machine.Specifications.Model.Specification specification)
    {
      _listeners.ForEach(listener => listener.OnSpecificationStart(specification));
    }

    public void OnSpecificationEnd(Machine.Specifications.Model.Specification specification, Machine.Specifications.Model.SpecificationVerificationResult result)
    {
      _listeners.ForEach(listener => listener.OnSpecificationEnd(specification,result));

    }
  }
}
