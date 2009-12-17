namespace Machine.Specifications.Reporting.Model
{
  public interface ISpecificationVisitor
  {
    void Visit(Run run);
    void Visit(Assembly assembly);
    void Visit(Concern concern);
    void Visit(Context context);
    void Visit(Specification specification);
  }
}
