namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    public interface IRemoteSpecificationRunListener
    {
        void OnAssemblyStart(string assemblyInfoXml);
        void OnAssemblyEnd(string assemblyInfoXml);
        void OnRunStart();
        void OnRunEnd();
        void OnContextStart(string contextInfoXml);
        void OnContextEnd(string contextInfoXml);
        void OnSpecificationStart(string specificationInfoXml);
        void OnSpecificationEnd(string specificationInfoXml, string resultXml);
        void OnFatalError(string exceptionResultXml);
    }
}