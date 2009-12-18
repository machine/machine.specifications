using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class NotImplementedSpecificationLinker : ISpecificationVisitor
  {
    ICanBeNotImplemented _firstNotImplemented;
    ICanBeNotImplemented _lastNotImplemented;

    public void Visit(Run run)
    {
      run.Assemblies.Each(Visit);

      ((ILinkToNotImplemented) run).Next = _firstNotImplemented;
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

      if (_lastNotImplemented != null)
      {
        ((ICanBeNotImplemented) specification).Previous = _lastNotImplemented;
        _lastNotImplemented.Next = specification;
      }

      _lastNotImplemented = specification;

      if (_firstNotImplemented == null)
      {
        _firstNotImplemented = specification;
      }
    }
  }
}