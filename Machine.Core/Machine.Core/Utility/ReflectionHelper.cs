using System;
using System.Collections.Generic;

namespace Machine.Core.Utility
{
  public static class ReflectionHelper
  {
    public static TType[] GetAttributes<TType>(Type type, bool inherit) where TType : Attribute
    {
      return (TType[])type.GetCustomAttributes(typeof(TType), inherit);
    }

    public static TType GetAttribute<TType>(Type type, bool inherit) where TType : Attribute
    {
      TType[] attributes = GetAttributes<TType>(type, inherit);
      if (attributes.Length == 0)
      {
        return null;
      }
      if (attributes.Length > 1)
      {
        throw new InvalidOperationException(String.Format("Only expecting one {0} on {1}", typeof(TType), type));
      }
      return attributes[0];
    }
  }
}
