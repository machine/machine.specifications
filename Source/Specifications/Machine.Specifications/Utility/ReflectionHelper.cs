using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Utility
{
  public static class ReflectionHelper
  {
    public static IEnumerable<FieldInfo> GetPrivateFields(this Type type)
    {
      return type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
    }

	public static IEnumerable<FieldInfo> GetPrivateOrInheritedFields(this Type type)
	{
	  return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
	}

    public static IEnumerable<FieldInfo> GetPrivateFieldsOfType<T>(this Type type)
    {
	  return type.GetPrivateFields().Where(x => x.FieldType == typeof(T));
    }

	public static IEnumerable<FieldInfo> GetPrivateOrInheritedFieldsOfType<T>(this Type type)
	{
	  return type.GetPrivateOrInheritedFields().Where(x => x.FieldType == typeof(T));
	}

    public static IEnumerable<FieldInfo> GetPrivateFieldsWith(this Type type, Type fieldType)
    {
      return type.GetPrivateFields().Where(x=>x.FieldType == fieldType);
    }

	public static IEnumerable<FieldInfo> GetPrivateOrInheritedFieldsWith(this Type type, Type fieldType)
	{
	  return type.GetPrivateOrInheritedFields().Where(x => x.FieldType == fieldType);
	}
  }
}