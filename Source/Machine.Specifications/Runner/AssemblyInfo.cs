using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class AssemblyInfo
  {
    public string Name { get; private set; }

    public AssemblyInfo(string name)
    {
      Name = name;
    }
  }
}
