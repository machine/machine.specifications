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

        public void OnAssemblyStart(string assemblyInfoXml)
        {
            this.listeners.Each(listener => listener.OnAssemblyStart(assemblyInfoXml));
        }

        public void OnAssemblyEnd(string assemblyInfoXml)
        {
            this.reversedListeners.Each(listener => listener.OnAssemblyEnd(assemblyInfoXml));
        }

        public void OnRunStart()
        {
            this.listeners.Each(listener => listener.OnRunStart());
        }

        public void OnRunEnd()
        {
            this.reversedListeners.Each(listener => listener.OnRunEnd());
        }

        public void OnContextStart(string contextInfoXml)
        {
            this.listeners.Each(listener => listener.OnContextStart(contextInfoXml));
        }

        public void OnContextEnd(string contextInfoXml)
        {
            this.reversedListeners.Each(listener => listener.OnContextEnd(contextInfoXml));
        }

        public void OnSpecificationStart(string specificationInfoXml)
        {
            this.listeners.Each(listener => listener.OnSpecificationStart(specificationInfoXml));
        }

        public void OnSpecificationEnd(string specificationInfoXml, string resultXml)
        {
            this.reversedListeners.Each(listener => listener.OnSpecificationEnd(specificationInfoXml, resultXml));
        }

        public void OnFatalError(string exceptionResultXml)
        {
            this.listeners.Each(listener => listener.OnFatalError(exceptionResultXml));
        }
    }
}