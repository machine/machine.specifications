namespace Machine.Specifications.Runner.Utility
{
    public interface ISpecificationRunListener
    {
        void OnAssemblyStart(AssemblyInfo assemblyInfo);

        void OnAssemblyEnd(AssemblyInfo assemblyInfo);

        void OnRunStart();

        void OnRunEnd();

        void OnContextStart(ContextInfo contextInfo);

        void OnContextEnd(ContextInfo contextInfo);

        void OnSpecificationStart(SpecificationInfo specificationInfo);

        void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result);

        void OnFatalError(ExceptionResult exceptionResult);
    }
}