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

      return !clazz.IsAbstract &&
        clazz.GetContainingType() == null &&
             clazz.GetAccessRights() == AccessRights.PUBLIC &&
             clazz.GetMembers().Any(x => IsSpecification(x) || IsBehavior(x));
    }

    public static bool IsSpecification(this IDeclaredElement element)
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

      return field.IsValid() && new CLRTypeName(fieldType.GetCLRName()) == new CLRTypeName(typeof(It).FullName);
    }

    public static bool IsBehavior(this IDeclaredElement element)
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

      return field.IsValid() && new CLRTypeName(fieldType.GetCLRName()) == new CLRTypeName(typeof(Behaves_like<>).FullName);
    }

    public static ICollection<string> GetTags(this IAttributesOwner type)
    {
      return type.GetAttributeInstances(new CLRTypeName(typeof(TagsAttribute).FullName), false)
        .SelectMany(x => x.PositionParameters(), (x, v) => v.ConstantValue.Value.ToString())
        .Distinct()
        .ToList();
    }

    public static bool IsIgnored(this IDeclaredElement type)
    {
      IAttributesOwner attributeOwner = type as IAttributesOwner;
      if (attributeOwner == null)
      {
        return false;
      }

      return attributeOwner.HasAttributeInstance(new CLRTypeName(typeof(IgnoreAttribute).FullName), false);
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