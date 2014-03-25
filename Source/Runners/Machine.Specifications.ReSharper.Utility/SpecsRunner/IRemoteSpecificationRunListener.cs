namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    public interface IRemoteSpecificationRunListener
    {
        void OnAssemblyStart(RemoteAssemblyInfo remoteAssemblyInfo);
        void OnAssemblyEnd(RemoteAssemblyInfo remoteAssembly);
        void OnRunStart();
        void OnRunEnd();
        void OnContextStart(RemoteContextInfo remoteContext);
        void OnContextEnd(RemoteContextInfo remoteContext);
        void OnSpecificationStart(RemoteSpecificationInfo remoteSpecification);
        void OnSpecificationEnd(RemoteSpecificationInfo remoteSpecification, RemoteResult remoteResult);
        void OnFatalError(RemoteExceptionResult exception);
    }

    public interface ISpecificationResultProvider
    {
        bool FailureOccurred { get; }
    }
}