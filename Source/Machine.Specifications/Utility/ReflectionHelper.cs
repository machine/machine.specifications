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

    public static IEnumerable<FieldInfo> GetInstanceFieldsOfUsage(this Type type, DelegateUsage usage)
    {
      return GetInstanceFields(type).Where(x => GetDelegateUsage(x) == usage);
    }

    public static IEnumerable<FieldInfo> GetInstanceFieldsOfUsage(this Type type, params DelegateUsage[] usages)
    {
      if (usages == null || usages.Length == 0) throw new ArgumentNullException("usages");
      var result = Enumerable.Empty<FieldInfo>();
      var fields = GetInstanceFields(type);
      foreach (var usage in usages)
      {
        result = result.Union(fields.Where(x => GetDelegateUsage(x) == usage));
      }
      return result;
    }

    public static FieldInfo GetStaticProtectedOrInheritedFieldNamed(this Type type, string fieldName)
    {
      return type.GetStaticProtectedOrInheritedFields().Where(x => x.Name == fieldName).SingleOrDefault();
    }

    public static bool IsOfUsage(this FieldInfo fieldInfo, DelegateUsage usage)
    {
      return GetDelegateUsage(fieldInfo) == usage;
    }

    public static DelegateUsage? GetDelegateUsage(this FieldInfo fieldInfo)
    {
      var fieldType = fieldInfo.FieldType;

      var attrs = fieldType.GetCustomAttributes(typeof(DelegateUsageAttribute), false);
      if (attrs.Length == 0) return null;
      var usage = ((DelegateUsageAttribute)attrs[0]).DelegateUsage;
      var invoke = fieldType.GetMethod("Invoke");

      // Do some validation to prevent messages with no indication of the cause of the problem later on.
      if (invoke == null) throw new InvalidOperationException(string.Format("The delegate type {0} does not have an invoke method.", fieldType));
      if (invoke.GetParameters().Length != 0)
        throw new InvalidOperationException(string.Format("{0} delegates require 0 parameters, {1} has {2}.", usage, fieldType, invoke.GetParameters().Length));

      switch (usage)
      {
        case DelegateUsage.Behavior:
          if (!fieldType.IsGenericType)
            throw new InvalidOperationException(string.Format("{0} delegates need to be generic types with 1 parameter. {1} is not a generic type.", usage, fieldType));
          if (fieldType.GetGenericArguments().Length != 1)
            throw new InvalidOperationException(string.Format("{0} delegates need to be generic types with 1 parameter. {1} has {2} parameters.", usage, fieldType, fieldType.GetGenericParameterConstraints().Length));
          break;
        case DelegateUsage.Setup:
        case DelegateUsage.Act:
        case DelegateUsage.Assert:
        case DelegateUsage.Cleanup:
          // Only the parameters need to be validated.
          break;
      }

      return usage;
    }
  }
}