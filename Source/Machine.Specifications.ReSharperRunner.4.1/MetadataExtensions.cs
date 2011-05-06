using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner
{
  internal static partial class MetadataExtensions
  {
    public static bool IsContext(this IMetadataTypeInfo type)
    {
      return !type.IsAbstract &&
             !type.IsStruct() &&
             (type.IsPublic || type.IsNestedPublic) &&
             type.GenericParameters.Length == 0 &&
             !type.HasCustomAttribute(typeof(BehaviorsAttribute).FullName) &&
             (type.GetSpecifications().Any() ||
              type.GetBehaviors().Any());
    }

    static bool IsStruct(this IMetadataTypeInfo type)
    {
      return type.Base.Type.FullyQualifiedName == typeof(ValueType).FullName;
    }

    public static IEnumerable<IMetadataField> GetSpecifications(this IMetadataTypeInfo type)
    {
      return type.GetPrivateFieldsOfType<It>();
    }

    public static IEnumerable<IMetadataField> GetBehaviors(this IMetadataTypeInfo type)
    {
      IEnumerable<IMetadataField> behaviorFields = type.GetPrivateFieldsWith(typeof(Behaves_like<>));
      foreach (IMetadataField field in behaviorFields)
      {
        if (field.GetFirstGenericArgument().HasCustomAttribute(typeof(BehaviorsAttribute).FullName)
#if !RESHARPER_5
            && field.GetFirstGenericArgument().GenericParameters.Length == 0      
#endif
            )
        {
          yield return field;
        }
      }
    }

    public static string GetSubjectString(this IMetadataTypeInfo type)
    {
      var attribute = type.AndAllBaseTypes()
        .SelectMany(x => x.GetCustomAttributes(typeof(SubjectAttribute).FullName))
        .FirstOrDefault();

      if (attribute == null)
      {
        if (type.DeclaringType == null)
        {
          return null;
        }

        return type.DeclaringType.GetSubjectString();
      }

      var parameters = attribute.ConstructorArguments.Select(x =>
                                    {
                                      var typeArgument = x as IMetadataClassType;
                                      if (typeArgument != null)
                                      {
                                        return new CLRTypeName(typeArgument.Type.FullyQualifiedName).ShortName;
                                      }

                                      return (string) x;
                                    })
                                  .ToArray();
      
      return String.Join(" ", parameters);
    }

    public static ICollection<string> GetTags(this IMetadataEntity type)
    {
      return type.AndAllBaseTypes()
        .SelectMany(x => x.GetCustomAttributes(typeof(TagsAttribute).FullName))
        .Select(x => x.ConstructorArguments)
        .Flatten(tag => tag.FirstOrDefault() as string,
                 tag => tag.Skip(1).FirstOrDefault() as IEnumerable<string>)
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
      var genericArgument = ((IMetadataClassType) genericField.Type).Arguments.First();
      return ((IMetadataClassType) genericArgument).Type;
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

        foreach (var coll in collectionResultSelector(s))
        {
          yield return coll;
        }
      }
    }

    static IEnumerable<IMetadataField> GetPrivateFields(this IMetadataTypeInfo type)
    {
      return type.GetFields().Where(field => !field.IsStatic);
    }

    static IEnumerable<IMetadataField> GetPrivateFieldsOfType<T>(this IMetadataTypeInfo type)
    {
      return type.GetPrivateFieldsWith(typeof(T));
    }

    static IEnumerable<IMetadataField> GetPrivateFieldsWith(this IMetadataTypeInfo type, Type fieldType)
    {
      return type.GetPrivateFields()
        .Where(x => x.Type is IMetadataClassType)
        .Where(x => new CLRTypeName(((IMetadataClassType) x.Type).Type.FullyQualifiedName) ==
                    new CLRTypeName(fieldType.FullName));
    }
  }
}
