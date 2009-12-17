using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Machine.Specifications.Reporting.Model
{
  public class Concern : SpecificationContainer, ISpecificationNode
  {
    readonly IEnumerable<Context> _contexts;
    readonly int _totalContexts;
    readonly string _name;

    public Concern(string name, IEnumerable<Context> contexts) : base(contexts.Cast<SpecificationContainer>())
    {
      _name = name;
      _contexts = contexts;
      _totalContexts = contexts.Count();
    }

    public string Name
    {
      get { return _name; }
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

    [JsonIgnore]
    public IEnumerable<ISpecificationNode> Children
    {
      get { return _contexts.Cast<ISpecificationNode>(); }
    }
  }
}
