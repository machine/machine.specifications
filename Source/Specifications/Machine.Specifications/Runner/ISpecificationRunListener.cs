using System.Reflection;

using Machine.Specifications.Model;

namespace Machine.Specifications.Runner
{
  public interface ISpecificationRunListener
  {
    void OnAssemblyStart(Assembly assembly);
    void OnAssemblyEnd(Assembly assembly);
    void OnRunStart();
    void OnRunEnd();
    void OnContextStart(Model.Context context);
    void OnContextEnd(Model.Context context);
    void OnSpecificationStart(Specification specification);
    void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result);
  }
}