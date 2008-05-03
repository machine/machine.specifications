using System;
using System.Collections.Generic;

namespace Machine.Core.ValueTypes
{
  public class ClassTypeAsValueType
  {
    public override bool Equals(object obj)
    {
      return ValueTypeHelper.AreEqual(this, obj);
    }

    public override int GetHashCode()
    {
      return ValueTypeHelper.CalculateHashCode(this);
    }

    public override string ToString()
    {
      return ValueTypeHelper.ToString(this);
    }
  }
}
