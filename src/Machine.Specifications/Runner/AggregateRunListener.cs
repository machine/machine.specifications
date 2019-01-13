using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner
{
    public class AggregateRunListener : ISpecificationRunListener
    {
        readonly IList<ISpecificationRunListener> _listeners;

        public AggregateRunListener(IEnumerable<ISpecificationRunListener> listeners)
        {
            _listeners = new List<ISpecificationRunListener>(listeners);
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            _listeners.Each(listener => listener.OnAssemblyStart(assembly));
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            _listeners.Reverse().Each(listener => listener.OnAssemblyEnd(assembly));
        }

        public void OnRunStart()
        {
            _listeners.Each(listener => listener.OnRunStart());
        }

        public void OnRunEnd()
        {
            _listeners.Reverse().Each(listener => listener.OnRunEnd());
        }

        public void OnContextStart(ContextInfo context)
        {
            _listeners.Each(listener => listener.OnContextStart(context));
        }

        public void OnContextEnd(ContextInfo context)
        {
            _listeners.Reverse().Each(listener => listener.OnContextEnd(context));
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            _listeners.Each(listener => listener.OnSpecificationStart(specification));
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _listeners.Reverse().Each(listener => listener.OnSpecificationEnd(specification, result));
        }

        public void OnFatalError(ExceptionResult exception)
        {
            _listeners.Each(listener => listener.OnFatalError(exception));
        }
    }
}