using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperRunner
{
  internal static partial class MetadataExtensions
  {
    public static bool IsContext(this IMetadataTypeInfo type)
    {
      return !type.IsAbstract &&
             !type.IsStruct() &&
             type.GenericParameters.Length == 0 &&
             !type.HasCustomAttribute(typeof(BehaviorsAttribute).FullName) &&
             (type.GetSpecifications().Any() ||
              type.GetBehaviors().Any());
    }

    static bool IsStruct(this IMetadataTypeInfo type)
    {
      if (type.Base != null)
      {
        return type.Base.Type.FullyQualifiedName == typeof(ValueType).FullName;
      }
      return false;
    }

    public static IEnumerable<IMetadataField> GetSpecifications(this IMetadataTypeInfo type)
    {
        var privateFieldsOfType = type.GetInstanceFieldsOfType<It>();
        return privateFieldsOfType;
    }

      public static IEnumerable<IMetadataField> GetBehaviors(this IMetadataTypeInfo type)
    {
      IEnumerable<IMetadataField> behaviorFields = type.GetInstanceFieldsOfType(typeof(Behaves_like<>));
      foreach (IMetadataField field in behaviorFields)
      {
        if (field.GetFirstGenericArgument().HasCustomAttribute(typeof(BehaviorsAttribute).FullName))
        {
          yield return field;
        }
      }
    }

    public static string GetSubjectString(this IMetadataEntity type)
    {
      var attributes = GetSubjectAttributes(type);
      if (attributes.Count != 1)
      {
        var asMember = type as IMetadataTypeMember;
        if (asMember != null && asMember.DeclaringType != null)
        {
          return asMember.DeclaringType.GetSubjectString();
        }
        return null;
      }

      var attribute = attributes.First();

      var parameters = attribute.ConstructorArguments.Select(x =>
      {
        var typeArgument = x.Value as IMetadataClassType;
        if (typeArgument != null)
        {
          return new ClrTypeName(typeArgument.Type.FullyQualifiedName).ShortName;
        }

        return (string)x.Value;
      }).ToArray();

      return String.Join(" ", parameters);
    }

    private static IList<IMetadataCustomAttribute> GetSubjectAttributes(IMetadataEntity type)
    {
      return (from customAttribute in type.CustomAttributes
              where customAttribute.AndAllBaseTypes().Any(i => i.FullyQualifiedName == typeof(SubjectAttribute).FullName)
              select customAttribute).ToList();
    }

    public static ICollection<string> GetTags(this IMetadataEntity type)
    { 
        return type.AndAllBaseTypes()
                .SelectMany(x => x.GetCustomAttributes(typeof(TagsAttribute).FullName))
                .Select(x => x.ConstructorArguments)
                .Flatten(tag => tag.FirstOrDefault().Value as string,
                tag => tag.Skip(1).FirstOrDefault().ValuesArray.Select(v => v.Value as string))
                .Distinct()
                .ToList();
    }

    static IEnumerable<IMetadataTypeInfo> AndAllBaseTypes(this IMetadataEntity type)
    {
      var typeInfo = type as IMetadataTypeInfo;
      if (typeInfo == null)
      {
        yield break;
      }

      yield return typeInfo;

      while (typeInfo.Base != null && typeInfo.Base.Type != null)
      {
        yield return typeInfo.Base.Type;

        typeInfo = typeInfo.Base.Type;
      }
    }

    public static IMetadataTypeInfo GetFirstGenericArgument(this IMetadataField genericField)
    {
      var genericArgument = ((IMetadataClassType)genericField.Type).Arguments.First();
      return ((IMetadataClassType)genericArgument).Type;
    }

    public static IMetadataClassType FirstGenericArgumentClass(this IMetadataField genericField)
    {
      var genericArgument = ((IMetadataClassType)genericField.Type).Arguments.First();
      return genericArgument as IMetadataClassType;
    }

    public static bool IsIgnored(this IMetadataEntity type)
    {
      return type.HasCustomAttribute(typeof(IgnoreAttribute).FullName);
    }

    static IEnumerable<TResult> Flatten<TSource, TResult>(this IEnumerable<TSource> source,
                                                          Func<TSource, TResult> singleResultSelector,
                                                          Func<TSource, IEnumerable<TResult>> collectionResultSelector)
    {
      foreach (var s in source)
      {
        yield return singleResultSelector(s);
          var resultSelector = collectionResultSelector(s);
          if (resultSelector == null)
          {
              yield break;
          }
           foreach (var coll in collectionResultSelector(s))
            {
                yield return coll;
            }
      }
    }

    static IEnumerable<IMetadataField> GetInstanceFields(this IMetadataTypeInfo type)
    {
      return type.GetFields().Where(field => !field.IsStatic);
    }

    static IEnumerable<IMetadataField> GetInstanceFieldsOfType<T>(this IMetadataTypeInfo type)
    {
      return type.GetInstanceFieldsOfType(typeof(T));
    }

    static IEnumerable<IMetadataField> GetInstanceFieldsOfType(this IMetadataTypeInfo type, Type fieldType)
    {
        var metadataFields = type.GetInstanceFields();
        var fields = metadataFields.Where(x => x.Type is IMetadataClassType);
        return fields.Where(x => (((IMetadataClassType)x.Type).Type.FullyQualifiedName == fieldType.FullName));
    }

    public static string FullyQualifiedName(this IMetadataClassType classType)
    {
      return FullyQualifiedName(classType, false);
    }

    static string FullyQualifiedName(this IMetadataClassType classType, bool appendAssembly)
    {
      var fullyQualifiedName = new StringBuilder();

      fullyQualifiedName.Append(classType.Type.FullyQualifiedName);

      if (classType.Arguments.Length > 0)
      {
        fullyQualifiedName.Append("[");
        fullyQualifiedName.Append(
          String.Join(",",
                      classType.Arguments
                        .Select(x => x as IMetadataClassType)
                        .Where(x => x != null)
                        .Select(x => "[" + x.FullyQualifiedName(true) + "]")
                        .ToArray()));
        fullyQualifiedName.Append("]");
      }

      if (appendAssembly)
      {
        fullyQualifiedName.Append(classType.AssemblyQualification);
      }

      return fullyQualifiedName.ToString();
    }
  }
}
