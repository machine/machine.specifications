using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class NotImplementedSpecificationLinker : ISpecificationVisitor
  {
    public const string Next = "Next_NotImplemented_Spec";
    public const string Previous = "Previous_NotImplemented_Spec";
    Specification _lastNotImplementedSpec;

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
      if (specification.Status != Status.NotImplemented)
      {
        return;
      }

      if (_lastNotImplementedSpec != null)
      {
        specification.Metadata[Previous] = _lastNotImplementedSpec.Metadata[SpecificationIdGenerator.Id];
        _lastNotImplementedSpec.Metadata[Next] = specification.Metadata[SpecificationIdGenerator.Id];
      }
      _lastNotImplementedSpec = specification;
    }
  }
}