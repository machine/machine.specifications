using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Model
{
    public class Context : SpecificationContainer, ISpecificationNode
    {
        readonly IEnumerable<Specification> _specifications;
        readonly string _name;
        readonly string _capturedOutput;

        public Context(string name, IEnumerable<Specification> specifications)
            : base(specifications)
        {
            _name = name;
            _specifications = specifications;
        }

        public Context(ContextInfo context, IEnumerable<Specification> specifications)
            : this(context.Name, specifications)
        {
            _capturedOutput = context.CapturedOutput;
        }

        public string Name
        {
            get { return _name; }
        }

        public string CapturedOutput
        {
            get { return _capturedOutput; }
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
