using System.Collections.Generic;

namespace Machine.Specifications.Model
{
  public class Behavior
  {
    readonly Context _context;
    readonly object _instance;
    readonly List<Specification> _specifications;

    public Behavior(object instance, Context context, bool isIgnored)
    {
      _instance = instance;
      _context = context;
      IsIgnored = isIgnored;
      _specifications = new List<Specification>();
    }

    public bool IsIgnored
    {
      get;
      private set;
    }

    public object Instance
    {
      get { return _instance; }
    }

    public IEnumerable<Specification> Specifications
    {
      get { return _specifications; }
    }

    public Context Context
    {
      get { return _context; }
    }

    public void AddSpecification(Specification specification)
    {
      _specifications.Add(specification);
    }
  }
}