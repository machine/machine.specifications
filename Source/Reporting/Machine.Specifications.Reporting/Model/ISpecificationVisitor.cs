using System;

namespace Machine.Specifications.Reporting.Model
{
  public interface ISpecificationVisitor
  {
    void Initialize(VisitorContext context);
    void Visit(Run run);
    void Visit(Assembly assembly);
    void Visit(Concern concern);
    void Visit(Context context);
    void Visit(Specification specification);
  }

  public class VisitorContext
  {
    public Func<string> ResourcePathCreator
    {
      get;
      set;
    }
  }
}
