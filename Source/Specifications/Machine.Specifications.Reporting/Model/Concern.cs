using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Reporting.Model
{
  public class Concern : SpecificationContainer, ISpecificationNode
  {
    readonly IEnumerable<Context> _contexts;
    readonly int _totalContexts;

    public Concern(IEnumerable<Context> contexts) : base(contexts.Cast<SpecificationContainer>())
    {
      _contexts = contexts;
      _totalContexts = contexts.Count();
    }

    public IEnumerable<Context> Contexts
    {
      get { return _contexts; }
    }

    public int TotalContexts
    {
      get { return _totalContexts; }
    }

    public void Accept(ISpecificationVisitor visitor)
    {
      visitor.Visit(this);
    }

    public IEnumerable<ISpecificationNode> Children
    {
      get { return _contexts.Cast<ISpecificationNode>(); }
    }
  }
}
