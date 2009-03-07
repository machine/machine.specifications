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
             !clazz.HasAttributeInstance(new CLRTypeName(typeof(BehaviorsAttribute).FullName), false) &&
             clazz.GetContainingType() == null &&
             clazz.GetAccessRights() == AccessRights.PUBLIC &&
             clazz.Fields.Any(x => IsSpecification(x) || IsBehavior(x));
    }

    public static bool IsSpecification(this IDeclaredElement element)
    {
      return element.IsValidFieldOfType(typeof(It));
    }

    public static bool IsBehavior(this IDeclaredElement element)
    {
      return element.IsValidFieldOfType(typeof(Behaves_like<>)) &&
             element.GetFirstGenericArgument() != null &&
             element.GetFirstGenericArgument().HasAttributeInstance(
               new CLRTypeName(typeof(BehaviorsAttribute).FullName),false);
    }

    public static IClass GetFirstGenericArgument(this IDeclaredElement element)
    {
      IDeclaredType fieldType = element.GetValidatedFieldType();
      if (fieldType == null)
      {
        return null;
      }

      var firstArgument = fieldType.GetSubstitution().Domain.First();
      var referencedType = fieldType.GetSubstitution().Apply(firstArgument).GetScalarType();

      if (referencedType != null)
      {
        return referencedType.GetTypeElement() as IClass;
      }

      return null;
    }

    public static IEnumerable<IField> GetBehaviorSpecifications(this IClass clazz)
    {
      return clazz.Fields.Where(IsSpecification);
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

    static bool IsValidFieldOfType(this IDeclaredElement element, Type type)
    {
      IDeclaredType fieldType = element.GetValidatedFieldType();
      if (fieldType == null)
      {
        return false;
      }

      return new CLRTypeName(fieldType.GetCLRName()) == new CLRTypeName(type.FullName);
    }

    static IDeclaredType GetValidatedFieldType(this IDeclaredElement element)
    {
      IField field = element as IField;
      if (field == null)
      {
        return null;
      }

      IDeclaredType fieldType = field.Type as IDeclaredType;
      if (fieldType == null)
      {
        return null;
      }

      if (!field.IsValid() ||
          !fieldType.IsResolved)
      {
        return null;
      }

      return fieldType;
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