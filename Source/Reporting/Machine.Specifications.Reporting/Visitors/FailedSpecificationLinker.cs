using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class FailedSpecificationLinker : ISpecificationVisitor
  {
    Specification _firstFail;
    Specification _lastFail;

    public void Initialize(VisitorContext context)
    {
    }

    public void Visit(Run run)
    {
      _firstFail = null;
      _lastFail = null;

      run.Assemblies.Each(Visit);

      run.NextFailed = _firstFail;
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
      if (IsSuccessful(specification))
      {
        return;
      }

      _lastFail = specification.LinkFailureTo(_lastFail);
      SetFirstFailure(specification);
    }

    static bool IsSuccessful(Specification specification)
    {
      return specification.Status != Status.Failing;
    }

    void SetFirstFailure(Specification specification)
    {
      if (_firstFail == null)
      {
        _firstFail = specification;
      }
    }
  }
}