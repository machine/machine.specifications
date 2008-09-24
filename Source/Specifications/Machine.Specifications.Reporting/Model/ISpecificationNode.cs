using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Reporting.Model
{
  public interface ISpecificationNode
  {
    void Accept(ISpecificationVisitor visitor);
    IEnumerable<ISpecificationNode> Children { get; }
  }
}