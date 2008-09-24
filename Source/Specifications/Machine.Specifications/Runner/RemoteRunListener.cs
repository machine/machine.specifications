using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class RemoteRunListener : MarshalByRefObject, ISpecificationRunListener
  {
    readonly ISpecificationRunListener _listener;

    public RemoteRunListener(ISpecificationRunListener listener)
    {
      _listener = listener;
    }

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      _listener.OnAssemblyStart(assembly);
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      _listener.OnAssemblyEnd(assembly);
    }

    public void OnRunStart()
    {
      _listener.OnRunStart();
    }

    public void OnRunEnd()
    {
      _listener.OnRunEnd();
    }

    public void OnContextStart(ContextInfo context)
    {
      _listener.OnContextStart(context);
    }

    public void OnContextEnd(ContextInfo context)
    {
      _listener.OnContextEnd(context);
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      _listener.OnSpecificationStart(specification);
    }

    public void OnSpecificationEnd(SpecificationInfo specification, SpecificationVerificationResult result)
    {
      _listener.OnSpecificationEnd(specification, result);
    }
  }
}
