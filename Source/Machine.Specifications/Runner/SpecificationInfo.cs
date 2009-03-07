using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class SpecificationInfo
  {
    public string Name { get; set; }
    public string ContainingType { get; set; }

    public SpecificationInfo(string name, string containingType)
    {
      Name = name;
      ContainingType = containingType;
    }
  }
}
