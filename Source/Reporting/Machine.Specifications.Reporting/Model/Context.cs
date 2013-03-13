using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Reporting.Model
{
  public class Context : SpecificationContainer, ISpecificationNode  
  {
    readonly IEnumerable<Specification> _specifications;
    readonly string _name;

    public Context(string name, IEnumerable<Specification> specifications) : base(specifications)
    {
      _name = name;
      _specifications = specifications;
    }

    public string Name
    {
      get { return _name; }
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
