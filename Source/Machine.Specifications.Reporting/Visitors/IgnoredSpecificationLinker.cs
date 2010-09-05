using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class IgnoredSpecificationLinker : ISpecificationVisitor
  {
    Specification _firstIgnored;
    Specification _lastIgnored;

    public void Initialize(VisitorContext context)
    {
    }

    public void Visit(Run run)
    {
      _firstIgnored = null;
      _lastIgnored = null;

      run.Assemblies.Each(Visit);

      run.NextIgnored = _firstIgnored;
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
      if (IsRun(specification))
      {
        return;
      }

      _lastIgnored = specification.LinkIgnoredTo(_lastIgnored);
      SetFirstNotImplemented(specification);
    }

    void SetFirstNotImplemented(Specification specification)
    {
      if (_firstIgnored == null)
      {
        _firstIgnored = specification;
      }
    }

    static bool IsRun(Specification specification)
    {
      return specification.Status != Status.Ignored;
    }
  }
}