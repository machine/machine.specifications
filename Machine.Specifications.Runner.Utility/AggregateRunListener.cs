using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Runner.Utility
{
    /// <summary>
    /// Aggregates several listeners and enebales to subscribe more than one listener. Its for example
    /// used to subscribe a TimerListener and a runListener like a PerAssemblyRunLitener
    /// </summary>
    public class AggregateRunListener : SpecificationRunListenerBase
    {
        private readonly IEnumerable<ISpecificationRunListener> reversedListeners;
        private readonly List<ISpecificationRunListener> listeners;

        public AggregateRunListener(IEnumerable<ISpecificationRunListener> listeners)
        {
            this.listeners = new List<ISpecificationRunListener>(listeners);
            this.reversedListeners = Enumerable.Reverse(this.listeners);
        }

        public override void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            this.listeners.Each(listener => listener.OnAssemblyStart(assemblyInfo));
        }

        public override void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            this.listeners.Each(listener => listener.OnAssemblyEnd(assemblyInfo));
        }

        public override void OnRunStart()
        {
            this.listeners.Each(listener => listener.OnRunStart());
        }

        public override void OnRunEnd()
        {
            this.reversedListeners.Each(listener => listener.OnRunEnd());
        }

        public override void OnContextStart(ContextInfo contextInfo)
        {
            this.listeners.Each(listener => listener.OnContextStart(contextInfo));
        }

        public override void OnContextEnd(ContextInfo contextInfo)
        {
            this.listeners.Each(listener => listener.OnContextEnd(contextInfo));
        }

        public override void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            this.listeners.Each(listener => listener.OnSpecificationStart(specificationInfo));
        }

        public override void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            this.reversedListeners.Each(listener => listener.OnSpecificationEnd(specificationInfo, result));
        }

        public override void OnFatalError(ExceptionResult exceptionResult)
        {
            this.listeners.Each(listener => listener.OnFatalError(exceptionResult));
        }
    }
}