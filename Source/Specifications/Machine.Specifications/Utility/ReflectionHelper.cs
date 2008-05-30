using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Utility
{
  public static class ReflectionHelper
  {
    public static IEnumerable<FieldInfo> GetPrivateFields(this Type type)
    {
      return type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
    }

    public static IEnumerable<FieldInfo> GetPrivateFieldsWith(this Type type, Type fieldType)
    {
      return type.GetPrivateFields().Where(x=>x.FieldType == fieldType);
    }
  }
}