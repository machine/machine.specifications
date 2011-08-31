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

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      if (obj.GetType() != typeof(ContextInfo))
      {
        return false;
      }
      return GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = 17;
        hash = hash * 29 + (Name != null ? Name.GetHashCode() : 0);
        hash = hash * 29 + (Concern != null ? Concern.GetHashCode() : 0);
        hash = hash * 29 + (TypeName != null ? TypeName.GetHashCode() : 0);
        hash = hash * 29 + (Namespace != null ? Namespace.GetHashCode() : 0);
        hash = hash * 29 + (AssemblyName != null ? AssemblyName.GetHashCode() : 0);

        return hash;
      }
    }
  }
}
