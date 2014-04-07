using System.Collections.Generic;

namespace Machine.Specifications.Reporting.Model
{
    public interface ISpecificationNode
    {
        void Accept(ISpecificationVisitor visitor);
        IEnumerable<ISpecificationNode> Children { get; }
    }
}