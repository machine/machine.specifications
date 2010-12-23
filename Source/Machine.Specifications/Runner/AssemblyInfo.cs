using System;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class AssemblyInfo
  {
    public AssemblyInfo(string name, string location)
    {
      Name = name;
      Location = location;
    }

    public string Name
    {
      get;
      private set;
    }

    public string Location
    {
      get;
      private set;
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
      if (obj.GetType() != typeof(AssemblyInfo))
      {
        return false;
      }
      return GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Location != null ? Location.GetHashCode() : 0);
      }
    }
  }
}