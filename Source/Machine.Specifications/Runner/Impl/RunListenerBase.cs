using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner.Impl
{
  public class RunListenerBase : ISpecificationRunListener
  {
    public virtual void OnAssemblyStart(AssemblyInfo assembly)
    {
    }

    public virtual void OnAssemblyEnd(AssemblyInfo assembly)
    {
    }

    public virtual void OnRunStart()
    {
    }

    public virtual void OnRunEnd()
    {
    }

    public virtual void OnContextStart(ContextInfo context)
    {
    }

    public virtual void OnContextEnd(ContextInfo context)
    {
    }

    public virtual void OnSpecificationStart(SpecificationInfo specification)
    {
    }

    public virtual void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
    }

    public virtual void OnFatalError(ExceptionResult exception)
    {
    }
  }
}
