using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Reporting.Model
{
  public enum SpecificationStatus
  {
    Passing,
    Failing,
    NotImplemented
  }

  public class Specification : ISpecificationNode
  {
    readonly SpecificationStatus _status;

    public Specification(SpecificationStatus _status)
    {
      this._status = _status;
    }

    public SpecificationStatus Status
    {
      get { return _status; }
    }

    public void Accept(ISpecificationVisitor visitor)
    {
      visitor.Visit(this);
    }

    public IEnumerable<ISpecificationNode> Children
    {
      get { yield break; }
    }
  }
}
