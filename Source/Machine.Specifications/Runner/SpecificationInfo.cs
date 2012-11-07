using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class SpecificationInfo
  {
    public string Leader { get; set; }
    public string Name { get; set; }
    public string ContainingType { get; set; }
    public string FieldName { get; set; }

    public SpecificationInfo(string leader, string name, string containingType, string fieldName)
    {
      Leader = leader;
      Name = name;
      ContainingType = containingType;
      FieldName = fieldName;
    }
  }
}
