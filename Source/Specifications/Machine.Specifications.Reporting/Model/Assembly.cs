using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Reporting.Model
{
  public class Assembly : SpecificationContainer, ISpecificationNode
  {
    readonly IEnumerable<Concern> _concerns;
    readonly int _totalConerns;
    readonly int _totalContexts;

    public Assembly(IEnumerable<Concern> concerns) : base(concerns.Cast<SpecificationContainer>())
    {
      _concerns = concerns;
      _totalConerns = concerns.Count();
      _totalContexts = concerns.Sum(x => x.TotalContexts);
    }

    public IEnumerable<Concern> Concerns
    {
      get { return _concerns; }
    }

    public int TotalConcerns
    {
      get { return _totalConerns; }
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
      get { return _concerns.Cast<ISpecificationNode>(); }
    }
  }
}
