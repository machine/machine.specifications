using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Machine.Specifications.Reporting.Model
{
  public class Context : SpecificationContainer, ISpecificationNode  
  {
    readonly IEnumerable<Specification> _specifications;

    public Context(IEnumerable<Specification> specifications) : base(specifications)
    {
      _specifications = specifications;
    }

    public IEnumerable<Specification> Specifications
    {
      get { return _specifications; }
    }

    public void Accept(ISpecificationVisitor visitor)
    {
      visitor.Visit(this);
    }

    public IEnumerable<ISpecificationNode> Children
    {
      get { return _specifications.Cast<ISpecificationNode>(); }
    }
  }
}
