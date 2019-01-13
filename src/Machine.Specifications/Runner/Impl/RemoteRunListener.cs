#if !NETSTANDARD

using System;
using System.Security;

namespace Machine.Specifications.Runner.Impl
{
    [Serializable]
    internal class RemoteRunListener : MarshalByRefObject, ISpecificationRunListener
    {
        readonly ISpecificationRunListener _listener;

        public RemoteRunListener(ISpecificationRunListener listener)
        {
            _listener = listener;
        }

        public void OnRunStart()
        {
            _listener.OnRunStart();
        }

        public void OnRunEnd()
        {
            _listener.OnRunEnd();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            _listener.OnAssemblyStart(assembly);
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            _listener.OnAssemblyEnd(assembly);
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

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _listener.OnSpecificationEnd(specification, result);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            _listener.OnFatalError(exception);
        }

        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
#endif
