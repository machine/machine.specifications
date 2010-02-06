using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace Machine.Specifications.ReSharperRunner
{
  internal static partial class PsiExtensions
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
    
    public static bool IsBehaviorContainer(this IDeclaredElement element)
    {
      var clazz = element as IClass;
      if (clazz == null)
      {
        return false;
      }

      return clazz.IsValid() &&
             !clazz.IsAbstract &&
             clazz.HasAttributeInstance(new CLRTypeName(typeof(BehaviorsAttribute).FullName), false) &&
             clazz.Fields.Any(IsSpecification);
    }

    public static bool IsSpecification(this IDeclaredElement element)
    {
      return element.IsValidFieldOfType(typeof(It));
    }

    public static bool IsSupportingField(this IDeclaredElement element)
    {
      return element.IsValidFieldOfType(typeof(Establish)) ||
             element.IsValidFieldOfType(typeof(Because)) ||
             element.IsValidFieldOfType(typeof(Cleanup));
    }

    public static bool IsField(this IDeclaredElement element)
    {
      return element is IField;
    }

    public static bool IsConstant(this IDeclaredElement element)
    {
      return (element.IsField() && ((IField)element).IsConstant) ||
             (element.IsLocal() && ((ILocalVariable)element).IsConstant);
    }

    public static bool IsLocal(this IDeclaredElement element)
    {
      return element is ILocalVariable;
    }

    public static bool IsBehavior(this IDeclaredElement element)
    {
      return element.IsValidFieldOfType(typeof(Behaves_like<>)) &&
             element.GetFirstGenericArgument() != null &&
             element.GetFirstGenericArgument().HasAttributeInstance(
               new CLRTypeName(typeof(BehaviorsAttribute).FullName), false);
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

      IInitializerOwnerDeclaration initializer = element as IInitializerOwnerDeclaration;
      bool hasInitializer = true;
      if (initializer != null)
      {
        hasInitializer = initializer.Initializer != null;
      }

      return !hasInitializer || attributeOwner.HasAttributeInstance(new CLRTypeName(typeof(IgnoreAttribute).FullName), false);
    }

    public static string GetSubjectString(this IAttributesOwner type)
    {
      var attribute = type.GetAttributeInstances(new CLRTypeName(typeof(SubjectAttribute).FullName), true)
                          .FirstOrDefault();

      if (attribute == null)
      {
        return null;
      }

      if (attribute.PositionParameters().Any(x => x.IsBadValue))
      {
        return null;
      }

      if (attribute.PositionParameters()
                   .Where(x => x.IsType)
                   .Select(x => x.TypeValue as IDeclaredType)
                   .Any(x => x.IsInvalid()))
      {
        return null;
      }

      try
      {
        var parameters = attribute.PositionParameters()
                                  .Select(x =>
                                    {
                                      if (x.IsType)
                                      {
                                        var declaredType = (IDeclaredType) x.TypeValue;
                                        return new CLRTypeName(declaredType.GetCLRName()).ShortName;
                                      }

                                      return (string) x.ConstantValue.Value;
                                    })
                                  .ToArray();

        return String.Join(" ", parameters);
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static ICollection<string> GetTags(this IAttributesOwner type)
    {
      return type.GetAttributeInstances(new CLRTypeName(typeof(TagsAttribute).FullName), true)
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
