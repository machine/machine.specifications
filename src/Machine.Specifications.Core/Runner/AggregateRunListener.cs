using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner
{
    public class AggregateRunListener : ISpecificationRunListener
    {
        private readonly IList<ISpecificationRunListener> listeners;

        public AggregateRunListener(IEnumerable<ISpecificationRunListener> listeners)
        {
            this.listeners = new List<ISpecificationRunListener>(listeners);
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            listeners.Each(listener => listener.OnAssemblyStart(assembly));
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            listeners.Reverse().Each(listener => listener.OnAssemblyEnd(assembly));
        }

        public void OnRunStart()
        {
            listeners.Each(listener => listener.OnRunStart());
        }

        public void OnRunEnd()
        {
            listeners.Reverse().Each(listener => listener.OnRunEnd());
        }

        public void OnContextStart(ContextInfo context)
        {
            listeners.Each(listener => listener.OnContextStart(context));
        }

        public void OnContextEnd(ContextInfo context)
        {
            listeners.Reverse().Each(listener => listener.OnContextEnd(context));
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            listeners.Each(listener => listener.OnSpecificationStart(specification));
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            listeners.Reverse().Each(listener => listener.OnSpecificationEnd(specification, result));
        }

        public void OnFatalError(ExceptionResult exception)
        {
            listeners.Each(listener => listener.OnFatalError(exception));
        }
    }
}
