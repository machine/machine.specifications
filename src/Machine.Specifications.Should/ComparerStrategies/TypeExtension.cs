using System;
using System.Reflection;

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