using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Machine.Specifications.Reporting.Model
{
  public class Specification : ISpecificationNode, ILinkTarget, ILinkToCanFail, ILinkToNotImplemented
  {
    readonly ExceptionResult _exception;
    readonly Guid _id;
    readonly string _name;
    readonly Status _status;
    readonly IDictionary<string, IDictionary<string, string>> _supplements;

    public Specification(string name, Result result)
    {
      _id = Guid.NewGuid();
      _status = result.Status;
      _exception = result.Exception;
      _supplements = result.Supplements;
      _name = name;
    }

    public Guid Id
    {
      get { return _id; }
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

    [JsonIgnore]
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
  }
}