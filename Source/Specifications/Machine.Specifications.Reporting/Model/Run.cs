using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Machine.Specifications.Reporting.Model
{
  public class Run : SpecificationContainer, ISpecificationNode
  {
    readonly IEnumerable<Assembly> _assemblies;
    readonly int _totalAssemblies;
    readonly int _totalConcerns;
    readonly int _totalContexts;

    public Run(IEnumerable<Assembly> assemblies) : base(assemblies.Cast<SpecificationContainer>())
    {
      _assemblies = assemblies;
      _totalAssemblies = assemblies.Count();
      _totalConcerns = assemblies.Sum(x => x.TotalConcerns);
      _totalContexts = assemblies.Sum(x => x.TotalContexts);
    }

    public IEnumerable<Assembly> Assemblies
    {
      get { return _assemblies; }
    }

    public int TotalAssemblies
    {
      get { return _totalAssemblies; }
    }

    public int TotalConcerns
    {
      get { return _totalConcerns; }
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
      get { return _assemblies.Cast<ISpecificationNode>(); }
    }
  }
}
