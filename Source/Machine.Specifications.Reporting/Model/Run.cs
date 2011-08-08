using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Reporting.Model
{
  public class Run : SpecificationContainer, ISpecificationNode, ILinkToCanFail, ILinkToNotImplemented, ILinkToIgnored
  {
    readonly IEnumerable<Assembly> _assemblies;
    readonly int _totalAssemblies;
    readonly int _totalConcerns;
    readonly int _totalContexts;

    public Run(IEnumerable<Assembly> assemblies) : base(assemblies.Cast<SpecificationContainer>())
    {
      Meta = new Meta { GeneratedAt = DateTime.Now };
      _assemblies = assemblies.OrderBy(x => x.Name);
      _totalAssemblies = assemblies.Count();
      _totalConcerns = assemblies.Sum(x => x.TotalConcerns);
      _totalContexts = assemblies.Sum(x => x.TotalContexts);
    }

    public Meta Meta
    {
      get;
      set;
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

    public IEnumerable<ISpecificationNode> Children
    {
      get { return _assemblies.Cast<ISpecificationNode>(); }
    }

    public ILinkTarget PreviousFailed
    {
      get;
      set;
    }

    public ILinkTarget NextFailed
    {
      get;
      set;
    }

    public ILinkTarget PreviousNotImplemented
    {
      get;
      set;
    }

    public ILinkTarget NextNotImplemented
    {
      get;
      set;
    }
    
    public ILinkTarget PreviousIgnored
    {
      get;
      set;
    }

    public ILinkTarget NextIgnored
    {
      get;
      set;
    }
  }
}