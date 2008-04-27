using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class Specification
  {
    private List<Requirement> _requirements;
    public Type Type { get; set; }
    public string Name { get; set; }
    public string WhenClause { get; set; }

    public IEnumerable<Requirement> Requirements
    {
      get { return _requirements; }
    }


    public Specification(IEnumerable<Requirement> requirements)
    {
      _requirements = new List<Requirement>();
      _requirements.AddRange(requirements);
    }
  }
}
