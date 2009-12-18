using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class NotImplementedSpecificationLinker : ISpecificationVisitor
  {
    Specification _firstNotImplemented;
    Specification _lastNotImplemented;

    public void Visit(Run run)
    {
      run.Assemblies.Each(Visit);

      run.NextNotImplemented = _firstNotImplemented;
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
      if (IsNotImplemented(specification))
      {
        return;
      }

      _lastNotImplemented = specification.LinkNotImplementedTo(_lastNotImplemented);
      SetFirstNotImplemented(specification);
    }

    void SetFirstNotImplemented(Specification specification)
    {
      if (_firstNotImplemented == null)
      {
        _firstNotImplemented = specification;
      }
    }

    static bool IsNotImplemented(Specification specification)
    {
      return specification.Status != Status.NotImplemented;
    }
  }
}