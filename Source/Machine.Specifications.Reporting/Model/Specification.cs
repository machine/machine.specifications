using System.Collections.Generic;

using Newtonsoft.Json;

namespace Machine.Specifications.Reporting.Model
{
  public class Specification : ISpecificationNode
  {
    readonly Status _status;
    readonly ExceptionResult _exception;
    readonly string _name;

    public Specification(string name, Result result)
    {
      _status = result.Status;
      _exception = result.Exception;
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

    public ExceptionResult Exception
    {
      get { return _exception; }
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
  }
}
