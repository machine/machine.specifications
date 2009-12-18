using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class FailedSpecificationLinker : ISpecificationVisitor
  {
    public const string Next = "Next_Failed_Spec";
    public const string Previous = "Previous_Failed_Spec";
    Specification _lastFailedSpec;

    public void Visit(Run run)
    {
      run.Assemblies.Each(Visit);
    }

    public void Visit(Assembly assembly)
    {
      assembly.Concerns.Each(Visit);
    }

    public void Visit(Concern concern)
    {
      concern.Contexts.Each(Visit);
    }

    public void Visit(Context context)
    {
      context.Specifications.Each(Visit);
    }

    public void Visit(Specification specification)
    {
      if (specification.Status != Status.Failing)
      {
        return;
      }

      if (_lastFailedSpec != null)
      {
        specification.Metadata[Previous] = _lastFailedSpec.Metadata[SpecificationIdGenerator.Id];
        _lastFailedSpec.Metadata[Next] = specification.Metadata[SpecificationIdGenerator.Id];
      }
      _lastFailedSpec = specification;
    }
  }
}