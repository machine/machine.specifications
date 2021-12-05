using System;

namespace Machine.Specifications.Comparers
{
    internal static class TypeExtension
    {
        public static bool IsNullable(this Type type)
        {
            return type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>));
        }
    }
}
