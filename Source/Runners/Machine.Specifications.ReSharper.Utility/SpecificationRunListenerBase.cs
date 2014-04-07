namespace Machine.Specifications.Runner.Utility
{
    public class SpecificationRunListenerBase : ISpecificationRunListener
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

        void ISpecificationRunListener.OnAssemblyStart(string assemblyInfoXml)
        {
            this.OnAssemblyStart(AssemblyInfo.Parse(assemblyInfoXml));
        }

        void ISpecificationRunListener.OnAssemblyEnd(string assemblyInfoXml)
        {
            this.OnAssemblyEnd(AssemblyInfo.Parse(assemblyInfoXml));
        }

        void ISpecificationRunListener.OnRunStart()
        {
            this.OnRunStart();
        }

        void ISpecificationRunListener.OnRunEnd()
        {
            this.OnRunEnd();
        }

        void ISpecificationRunListener.OnContextStart(string contextInfoXml)
        {
            this.OnContextStart(ContextInfo.Parse(contextInfoXml));
        }

        void ISpecificationRunListener.OnContextEnd(string contextInfoXml)
        {
            this.OnContextEnd(ContextInfo.Parse(contextInfoXml));
        }

        void ISpecificationRunListener.OnSpecificationStart(string specificationInfoXml)
        {
            this.OnSpecificationStart(SpecificationInfo.Parse(specificationInfoXml));
        }

        void ISpecificationRunListener.OnSpecificationEnd(string specificationInfoXml, string resultXml)
        {
            this.OnSpecificationEnd(SpecificationInfo.Parse(specificationInfoXml), Result.Parse(resultXml));
        }

        void ISpecificationRunListener.OnFatalError(string exceptionResultXml)
        {
            this.OnFatalError(ExceptionResult.Parse(exceptionResultXml));
        }
    }
}
