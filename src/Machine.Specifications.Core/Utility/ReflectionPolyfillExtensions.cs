using System;
using System.Reflection;

namespace Machine.Specifications.Utility
{
    internal static class ReflectionCompatExtensions
    {
#if NET6_0_OR_GREATER
        public static bool IsType(this MemberInfo memberInfo)
        {
            return memberInfo is TypeInfo;
        }

        public static Type AsType(this MemberInfo memberInfo)
        {
             return ((TypeInfo)memberInfo).AsType();
        }

        public static string TryGetLocation(this Assembly assembly)
        {
            return assembly.Location;
        }
#else
        public static Type AsType(this MemberInfo memberInfo)
        {
             return (Type)memberInfo;
        }

        public static bool IsType(this MemberInfo memberInfo)
        {
            return memberInfo is Type;
        }

        public static string TryGetLocation(this Assembly assembly)
        {
            return assembly.Location;
        }
#endif
    }
}
