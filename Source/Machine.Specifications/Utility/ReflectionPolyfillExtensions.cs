
using System;
using System.Reflection;

#if NET35 || NET40

namespace System.Reflection
{
    internal static class SystemReflectionPolyfillExtensions
    {
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
    }
}
#endif


namespace Machine.Specifications.Utility
{
    internal static class ReflectionCompatExtensions
    {
#if NETSTANDARD
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
            // .NET Standard 1.3 doesn't support "Assembly.Location"
            // and UAP/UWP are 1.3.
            // However .Net Standard 1.5+ support it
            return null;
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