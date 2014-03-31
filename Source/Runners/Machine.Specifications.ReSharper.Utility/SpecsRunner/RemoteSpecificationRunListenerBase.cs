namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    public class RemoteSpecificationRunListenerBase : IRemoteSpecificationRunListener
    {
        protected virtual void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
        }

        protected virtual void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
        }

        protected virtual void OnRunStart()
        {
        }

        protected virtual void OnRunEnd()
        {
        }

        protected virtual void OnContextStart(ContextInfo contextInfo)
        {
        }

        protected virtual void OnContextEnd(ContextInfo contextInfo)
        {
        }

        protected virtual void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
        }

        protected virtual void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
        }

        protected virtual void OnFatalError(ExceptionResult exceptionResult)
        {
        }

        void IRemoteSpecificationRunListener.OnAssemblyStart(string assemblyInfoXml)
        {
            this.OnAssemblyStart(AssemblyInfo.Parse(assemblyInfoXml));
        }

        void IRemoteSpecificationRunListener.OnAssemblyEnd(string assemblyInfoXml)
        {
            this.OnAssemblyEnd(AssemblyInfo.Parse(assemblyInfoXml));
        }

        void IRemoteSpecificationRunListener.OnRunStart()
        {
            this.OnRunStart();
        }

        void IRemoteSpecificationRunListener.OnRunEnd()
        {
            this.OnRunEnd();
        }

        void IRemoteSpecificationRunListener.OnContextStart(string contextInfoXml)
        {
            this.OnContextStart(ContextInfo.Parse(contextInfoXml));
        }

        void IRemoteSpecificationRunListener.OnContextEnd(string contextInfoXml)
        {
            this.OnContextEnd(ContextInfo.Parse(contextInfoXml));
        }

        void IRemoteSpecificationRunListener.OnSpecificationStart(string specificationInfoXml)
        {
            this.OnSpecificationStart(SpecificationInfo.Parse(specificationInfoXml));
        }

        void IRemoteSpecificationRunListener.OnSpecificationEnd(string specificationInfoXml, string resultXml)
        {
            this.OnSpecificationEnd(SpecificationInfo.Parse(specificationInfoXml), Result.Parse(resultXml));
        }

        void IRemoteSpecificationRunListener.OnFatalError(string exceptionResultXml)
        {
            this.OnFatalError(ExceptionResult.Parse(exceptionResultXml));
        }
    }
}
