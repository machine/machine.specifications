using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class Concern
  {
    readonly string _description;
    readonly Type _type;

    public Type Type
    {
      get { return _type; }
    }

    public string Context
    {
      get { return _description; }
    }

    public Concern(Type type, string description)
    {
      _type = type;
      _description = description;
    }
  }
}
