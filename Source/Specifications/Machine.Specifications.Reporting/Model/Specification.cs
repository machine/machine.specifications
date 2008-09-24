using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Reporting.Model
{
  public class Specification : ISpecificationNode
  {
    readonly Status _status;
    readonly string _name;

    public Specification(string name, Status status)
    {
      _status = status;
      _name = name;
    }

    public Status Status
    {
      get { return _status; }
    }

    public string Name
    {
      get { return _name; }
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
