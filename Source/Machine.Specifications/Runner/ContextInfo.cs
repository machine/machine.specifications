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
    public string TypeName { get; private set; }
    public string Namespace { get; private set; }
    public string AssemblyName { get; private set; }
    public string FullName
    {
      get
      {
        string line = "";

        if (!String.IsNullOrEmpty(Concern))
        {
          line += Concern + ", ";
        }

        return line + Name;
      }
    }

    public ContextInfo(string name, string concern, string typeName, string @namespace, string assemblyName)
    {
      Concern = concern;
      Name = name;
      TypeName = typeName;
      AssemblyName = assemblyName;
      Namespace = @namespace;
    }
  }
}
