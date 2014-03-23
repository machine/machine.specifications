using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;

using Machine.Specifications.Sdk;

using CLRTypeName = JetBrains.ReSharper.Psi.ClrTypeName;

using JetBrains.ReSharper.Psi.Util;

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

      var fields = clazz.Fields;
      if (fields == null || !fields.Any())
      {
        return false;
      }

      return clazz.IsValid() &&
             !clazz.IsAbstract &&
             !clazz.HasAttributeInstance(new CLRTypeName(new BehaviorAttributeFullName()), false) &&
             fields.Any(x => IsSpecification(x) || IsBehavior(x));
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
             clazz.HasAttributeInstance(new CLRTypeName(new BehaviorAttributeFullName()), false) &&
             clazz.GetFirstGenericArgument() == null &&
             clazz.Fields.Any(IsSpecification);
    }

    public static bool IsSpecification(this IDeclaredElement element)
    {
      return element.IsValidFieldOfType(new AssertDelegateAttributeFullName());
    }

    public static bool IsSupportingField(this IDeclaredElement element)
    {
      return element.IsValidFieldOfType(new SetupDelegateAttributeFullName()) ||
             element.IsValidFieldOfType(new ActDelegateAttributeFullName()) ||
             element.IsValidFieldOfType(new CleanupDelegateAttributeFullName());
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
      return element.IsValidFieldOfType(new BehaviorDelegateAttributeFullName()) &&
             element.GetFirstGenericArgument() != null &&
             element.GetFirstGenericArgument().GetFirstGenericArgument() == null &&
             element.GetFirstGenericArgument().HasAttributeInstance(
               new CLRTypeName(new BehaviorAttributeFullName()), false);
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

      return !hasInitializer || attributeOwner.HasAttributeInstance(new CLRTypeName(new IgnoreAttributeFullName()), false);
    }

    public static string GetSubjectString(this IAttributesOwner type)
    {
      var attribute = GetSubjectAttribute(type);
      if (attribute == null)
      {
        var containingType = type.GetContainingType();
        if (containingType == null)
        {
          return null;
        }

        return containingType.GetSubjectString();
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
                                        return declaredType.GetClrName().ShortName;
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

    private static IAttributeInstance GetSubjectAttribute(IAttributesSet type)
    {
      return (from a in type.GetAttributeInstances(true)
              let h = (new[] { a.GetAttributeType() }).Concat(a.GetAttributeType().GetSuperTypes())
              where h.Any(t => t.GetClrName().FullName == new SubjectAttributeFullName())
              select a).FirstOrDefault();
    }

    public static ICollection<string> GetTags(this IAttributesOwner type)
    {
      return type.GetAttributeInstances(new CLRTypeName(new TagsAttributeFullName()), true)
        .SelectMany(x => x.PositionParameters())
        .SelectMany(x =>
        {
          if (x.IsArray)
          {
            return x.ArrayValue.Select(av => av.ConstantValue);
          }

          return new[] {x.ConstantValue};
        })
        .Select(v => v.Value.ToString())
        .Distinct()
        .ToList();
    }

    static bool IsValidFieldOfType(this IDeclaredElement element, string typeFullName)
    {
      IDeclaredType fieldType = element.GetValidatedFieldType();
      if (fieldType == null)
      {
        return false;
      }

      return fieldType.GetTypeElement().HasAttributeInstance(new ClrTypeName(typeFullName), false);
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

    static bool IsInvalid(this IExpressionType type)
    {
      return type == null || !type.IsValid();
    }

    public static bool IsContextBaseClass(this IDeclaredElement element)
    {
      var clazz = element as IClass;
      if (clazz == null)
      {
        return false;
      }

      var contextBaseCandidate = !element.IsContext() &&
                                 clazz.IsValid() &&
                                 clazz.GetContainingType() == null;
      if (!contextBaseCandidate)
      {
        return false;
      }

      IFinder finder = clazz.GetSolution().GetPsiServices().Finder;
      var searchDomain = clazz.GetSearchDomain();

      var findResult = new InheritedContextFinder();

      finder.FindInheritors(clazz,
                            searchDomain,
                            findResult.Consumer,
                            NullProgressIndicator.Instance);

      return findResult.Found;
    }

    class InheritedContextFinder
    {
      public InheritedContextFinder()
      {
        Found = false;

        Consumer = new FindResultConsumer(result =>
        {
          FindResultDeclaredElement foundElement = result as FindResultDeclaredElement;
          if (foundElement != null)
          {
            Found = foundElement.DeclaredElement.IsContext();
          }

          return Found ? FindExecution.Stop : FindExecution.Continue;
        });
      }

      public bool Found
      {
        get;
        private set;
      }

      public FindResultConsumer Consumer
      {
        get;
        private set;
      }
    }
  }
}
