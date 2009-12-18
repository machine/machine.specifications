using System;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class SpecificationIdGenerator : ISpecificationVisitor
  {
    public const string Id = "Spec_Id";

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
      specification.Metadata[Id] = Guid.NewGuid();
    }
  }
}