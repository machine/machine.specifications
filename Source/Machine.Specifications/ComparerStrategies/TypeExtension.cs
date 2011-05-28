using System;

namespace Machine.Specifications.ComparerStrategies
{
  static class TypeExtension
  {
    public static bool IsNullable(this Type type)
    {
      return type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>));
    }
  }
}