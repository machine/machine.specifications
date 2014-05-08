namespace Machine.Specifications.Runner
{
    public interface ISpecificationRunListener
    {
        void OnAssemblyStart(AssemblyInfo assembly);
        void OnAssemblyEnd(AssemblyInfo assembly);
        void OnRunStart();
        void OnRunEnd();
        void OnContextStart(ContextInfo context);
        void OnContextEnd(ContextInfo context);
        void OnSpecificationStart(SpecificationInfo specification);
        void OnSpecificationEnd(SpecificationInfo specification, Result result);
        void OnFatalError(ExceptionResult exception);
    }

    [ObsoleteEx(Message = "Information can be obtained by implementing ISpecificationRunListener.OnFatalError and or counting specifications with failures", TreatAsErrorFromVersion = "0.9", RemoveInVersion = "1.0")]
    public interface ISpecificationResultProvider
    {
        bool FailureOccurred { get; }
    }
}