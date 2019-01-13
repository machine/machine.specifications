using System;

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

    [Obsolete("Information can be obtained by implementing ISpecificationRunListener.OnFatalError and or counting specifications with failures. To be removed in 1.0.")]
    public interface ISpecificationResultProvider
    {
        bool FailureOccurred { get; }
    }
}