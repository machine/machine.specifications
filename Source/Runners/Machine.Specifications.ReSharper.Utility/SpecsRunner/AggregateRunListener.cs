namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Aggregates several listeners and enebales to subscribe more than one listener. Its for example
    /// used to subscribe a TimerListener and a runListener like a PerAssemblyRunLitener
    /// </summary>
    public class AggregateRunListener : IRemoteSpecificationRunListener
    {
        readonly IList<IRemoteSpecificationRunListener> _listeners;

        public AggregateRunListener(IEnumerable<IRemoteSpecificationRunListener> listeners)
        {
            this._listeners = new List<IRemoteSpecificationRunListener>(listeners);
        }

        public void OnAssemblyStart(RemoteAssemblyInfo assembly)
        {
            this._listeners.Each(listener => listener.OnAssemblyStart(assembly));
        }

        public void OnAssemblyEnd(RemoteAssemblyInfo assembly)
        {
            this._listeners.Reverse().Each(listener => listener.OnAssemblyEnd(assembly));
        }

        public void OnRunStart()
        {
            this._listeners.Each(listener => listener.OnRunStart());
        }

        public void OnRunEnd()
        {
            this._listeners.Reverse().Each(listener => listener.OnRunEnd());
        }

        public void OnContextStart(RemoteContextInfo context)
        {
            this._listeners.Each(listener => listener.OnContextStart(context));
        }

        public void OnContextEnd(RemoteContextInfo context)
        {
            this._listeners.Reverse().Each(listener => listener.OnContextEnd(context));
        }

        public void OnSpecificationStart(RemoteSpecificationInfo specification)
        {
            this._listeners.Each(listener => listener.OnSpecificationStart(specification));
        }

        public void OnSpecificationEnd(RemoteSpecificationInfo specification, RemoteResult result)
        {
            this._listeners.Reverse().Each(listener => listener.OnSpecificationEnd(specification, result));
        }

        public void OnFatalError(RemoteExceptionResult exception)
        {
            this._listeners.Each(listener => listener.OnFatalError(exception));
        }
    }
}