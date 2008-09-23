using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class ContextInfo
  {
    public string Name { get; private set; }
    public string Concern { get; private set; }
    public string FullName
    {
      get
      {
        string line = "";

        if (Concern != null)
        {
          line += Concern + ", ";
        }

        return line + Name;
      }
    }

    public ContextInfo(string name, string concern)
    {
      Concern = concern;
      Name = name;
    }
  }
}
