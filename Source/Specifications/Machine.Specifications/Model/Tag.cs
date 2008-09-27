using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class Tag
  {
    readonly Type type;
    public string Name { get { return type.Name; } }

    public Tag(Type type)
    {
      this.type = type;
    }

    public bool Equals(Tag obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return Equals(obj.type, type);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(Tag)) return false;
      return Equals((Tag) obj);
    }

    public override int GetHashCode()
    {
      return (type != null ? type.GetHashCode() : 0);
    }
  }
}
