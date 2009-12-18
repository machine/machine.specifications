using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class FailedSpecificationLinker : ISpecificationVisitor
  {
    ICanFail _firstFail;
    ICanFail _lastFail;

    public void Visit(Run run)
    {
      run.Assemblies.Each(Visit);

      ((ILinkToCanFail) run).Next = _firstFail;
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

      if (_lastFail != null)
      {
        ((ICanFail) specification).Previous = _lastFail;
        _lastFail.Next = specification;
      }

      _lastFail = specification;

      if (_firstFail == null)
      {
        _firstFail = specification;
      }
    }
  }
}