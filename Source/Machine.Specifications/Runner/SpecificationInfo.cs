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
    public string FieldName { get; set; }

    public SpecificationInfo(string name, string containingType, string fieldName)
    {
      Name = name;
      ContainingType = containingType;
      FieldName = fieldName;
    }

    //public override bool Equals(object obj)
    //{
    //    if (ReferenceEquals(null, obj))
    //    {
    //        return false;
    //    }
    //    if (ReferenceEquals(this, obj))
    //    {
    //        return true;
    //    }
    //    if (obj.GetType() != typeof(SpecificationInfo))
    //    {
    //        return false;
    //    }
    //    return GetHashCode() == obj.GetHashCode();
    //}

    //public override int GetHashCode()
    //{
    //    unchecked
    //    {
    //        int hash = 17;
    //        hash = hash * 29 + (Name != null ? Name.GetHashCode() : 0);
    //        hash = hash * 29 + (ContainingType != null ? ContainingType.GetHashCode() : 0);
    //        hash = hash * 29 + (FieldName != null ? FieldName.GetHashCode() : 0);

    //        return hash;
    //    }
    //}
  }
}
