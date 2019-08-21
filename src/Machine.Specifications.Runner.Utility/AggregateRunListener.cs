using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Runner.Utility
{
    /// <summary>
    /// Aggregates several listeners and enebales to subscribe more than one listener. Its for example
    /// used to subscribe a TimerListener and a runListener like a PerAssemblyRunLitener
    /// </summary>
    public class AggregateRunListener : ISpecificationRunListener
    {
        private readonly IEnumerable<ISpecificationRunListener> reversedListeners;
        private readonly List<ISpecificationRunListener> listeners;

        public AggregateRunListener(IEnumerable<ISpecificationRunListener> listeners)
        {
            this.listeners = new List<ISpecificationRunListener>(listeners);
            this.reversedListeners = Enumerable.Reverse(this.listeners);
        }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            this.listeners.Each(listener => listener.OnAssemblyStart(assemblyInfo));
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            this.listeners.Each(listener => listener.OnAssemblyEnd(assemblyInfo));
        }

        public void OnRunStart()
        {
            this.listeners.Each(listener => listener.OnRunStart());
        }

        public void OnRunEnd()
        {
            this.reversedListeners.Each(listener => listener.OnRunEnd());
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            this.listeners.Each(listener => listener.OnContextStart(contextInfo));
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            this.listeners.Each(listener => listener.OnContextEnd(contextInfo));
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            this.listeners.Each(listener => listener.OnSpecificationStart(specificationInfo));
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            this.reversedListeners.Each(listener => listener.OnSpecificationEnd(specificationInfo, result));
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            this.listeners.Each(listener => listener.OnFatalError(exceptionResult));
        }
    }
}