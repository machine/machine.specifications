using System.Reflection;

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

  public interface ISpecificationResultProvider
  {
    bool FailureOccurred { get; }
  }
}