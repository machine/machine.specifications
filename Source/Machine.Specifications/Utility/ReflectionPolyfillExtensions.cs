
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Machine.Specifications.Utility
{
    public static class ReflectionPolyfillExtensions
    {

#if NET35 || NET40
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif

#if NETSTANDARD
        public static Type AsType(this MemberInfo memberInfo)
        {
             return ((TypeInfo)memberInfo).AsType();
        }
#else
        public static Type AsType(this MemberInfo memberInfo)
        {
             return (Type)memberInfo;
        }
#endif
    }
}