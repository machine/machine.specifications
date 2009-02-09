using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner
{
  internal static class PsiExtensions
  {
    public static bool IsContext(this IDeclaredElement element)
    {
      var clazz = element as IClass;
      if (clazz == null)
      {
        return false;
      }

      return clazz.IsValid() &&
             !clazz.IsAbstract &&
             clazz.GetContainingType() == null &&
             clazz.GetAccessRights() == AccessRights.PUBLIC &&
             clazz.GetMembers().Any(x => IsSpecification(x) || IsBehavior(x));
    }

    public static bool IsSpecification(this IDeclaredElement element)
    {
      return element.IsOfType(typeof(It));
    }

    public static bool IsBehavior(this IDeclaredElement element)
    {
      return element.IsOfType(typeof(Behaves_like<>));
    }

    public static bool IsIgnored(this IDeclaredElement element)
    {
      IAttributesOwner attributeOwner = element as IAttributesOwner;
      if (attributeOwner == null)
      {
        return false;
      }

      return attributeOwner.HasAttributeInstance(new CLRTypeName(typeof(IgnoreAttribute).FullName), false);
    }

    public static ICollection<string> GetTags(this IAttributesOwner type)
    {
      return type.GetAttributeInstances(new CLRTypeName(typeof(TagsAttribute).FullName), false)
        .SelectMany(x => x.PositionParameters(), (x, v) => v.ConstantValue.Value.ToString())
        .Distinct()
        .ToList();
    }

    static bool IsOfType(this IDeclaredElement element, Type type)
    {
      var field = element as IField;
      if (field == null)
      {
        return false;
      }

      var fieldType = field.Type as IDeclaredType;
      if (fieldType == null)
      {
        return false;
      }

      return field.IsValid() &&
             fieldType.IsResolved &&
             new CLRTypeName(fieldType.GetCLRName()) == new CLRTypeName(type.FullName);
    }

    static IEnumerable<AttributeValue> PositionParameters(this IAttributeInstance source)
    {
      for (int i = 0; i < source.PositionParameterCount; i++)
      {
        yield return source.PositionParameter(i);
      }
    }
  }
}