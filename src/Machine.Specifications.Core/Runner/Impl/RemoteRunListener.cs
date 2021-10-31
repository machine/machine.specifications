#if !NETSTANDARD
using System;
using System.Security;

namespace Machine.Specifications.Runner.Impl
{
    [Serializable]
    internal class RemoteRunListener : MarshalByRefObject, ISpecificationRunListener
    {
        private readonly ISpecificationRunListener listener;

        public RemoteRunListener(ISpecificationRunListener listener)
        {
            this.listener = listener;
        }

        public void OnRunStart()
        {
            listener.OnRunStart();
        }

        public void OnRunEnd()
        {
            listener.OnRunEnd();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            listener.OnAssemblyStart(assembly);
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            listener.OnAssemblyEnd(assembly);
        }

        public void OnContextStart(ContextInfo context)
        {
            listener.OnContextStart(context);
        }

        public void OnContextEnd(ContextInfo context)
        {
            listener.OnContextEnd(context);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            listener.OnSpecificationStart(specification);
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            listener.OnSpecificationEnd(specification, result);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            listener.OnFatalError(exception);
        }

        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
#endif
