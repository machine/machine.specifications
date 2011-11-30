using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Utility
{
  public static class ReflectionHelper
  {
    public static IEnumerable<FieldInfo> GetInstanceFields(this Type type)
    {
      return type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);
    }

    public static IEnumerable<FieldInfo> GetStaticProtectedOrInheritedFields(this Type type)
    {
      return type
        .GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
        .Where(x => !x.IsPrivate);
    }

    public static IEnumerable<FieldInfo> GetInstanceFieldsOfType<T>(this Type type)
    {
      return GetInstanceFields(type).Where(x => x.FieldType == typeof(T));
    }

    public static IEnumerable<FieldInfo> GetInstanceFieldsOfType(this Type type, Type fieldType)
    {
      return GetInstanceFields(type).Where(x => x.FieldType.IsOfType(fieldType));
    }

    public static FieldInfo GetStaticProtectedOrInheritedFieldNamed(this Type type, string fieldName)
    {
      return type.GetStaticProtectedOrInheritedFields().Where(x => x.Name == fieldName).SingleOrDefault();
    }

    public static bool IsOfType(this Type type, Type fieldType)
    {
      if (type.IsGenericType)
      {
        return type.GetGenericTypeDefinition() == fieldType;
      }
      return type == fieldType;
    }
  }
}