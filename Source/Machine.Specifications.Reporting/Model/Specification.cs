using System.Collections.Generic;

namespace Machine.Specifications.Reporting.Model
{
  public class Specification : ISpecificationNode, ILinkTarget, ILinkToCanFail, ILinkToNotImplemented, ILinkToIgnored
  {
    readonly ExceptionResult _exception;
    readonly string _name;
    readonly Status _status;
    readonly IDictionary<string, IDictionary<string, string>> _supplements;

    public Specification(string name, Result result)
    {
      _status = result.Status;
      _exception = result.Exception;
      _supplements = result.Supplements;
      _name = name;
    }

    public string Id
    {
      get;
      set;
    }

    public Status Status
    {
      get { return _status; }
    }

    public string Name
    {
      get { return _name; }
    }

    public ExceptionResult Exception
    {
      get { return _exception; }
    }

    public IDictionary<string, IDictionary<string, string>> Supplements
    {
      get { return _supplements; }
    }

    public void Accept(ISpecificationVisitor visitor)
    {
      visitor.Visit(this);
    }

    public IEnumerable<ISpecificationNode> Children
    {
      get { yield break; }
    }

    public ILinkTarget NextFailed
    {
      get;
      set;
    }

    public ILinkTarget PreviousFailed
    {
      get;
      set;
    }

    public ILinkTarget NextNotImplemented
    {
      get;
      set;
    }

    public ILinkTarget PreviousNotImplemented
    {
      get;
      set;
    }

    public ILinkTarget NextIgnored
    {
      get;
      set;
    }

    public ILinkTarget PreviousIgnored
    {
      get;
      set;
    }
  }
}