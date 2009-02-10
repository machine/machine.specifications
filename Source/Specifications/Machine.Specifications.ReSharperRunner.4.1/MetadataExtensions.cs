using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner
{
  internal static class MetadataExtensions
  {
    public static bool IsContext(this IMetadataTypeInfo type)
    {
      return !type.IsAbstract &&
             type.IsPublic &&
             type.GenericParameters.Length == 0 &&
             (type.GetSpecifications().Any() ||
              type.GetBehaviors().Any());
    }

    public static IEnumerable<IMetadataField> GetSpecifications(this IMetadataTypeInfo type)
    {
      return type.GetPrivateFieldsOfType<It>();
    }

    public static IEnumerable<IMetadataField> GetBehaviors(this IMetadataTypeInfo type)
    {
      return type.GetPrivateFieldsWith(typeof(Behaves_like<>));
    }

    public static ICollection<string> GetTags(this IMetadataEntity type)
    {
      return type.GetCustomAttributes(typeof(TagsAttribute).FullName)
        .Select(x => x.ConstructorArguments)
        .Flatten(tag => tag.FirstOrDefault() as string,
                 tag => tag.Skip(1).FirstOrDefault() as IEnumerable<string>)
        .Distinct()
        .ToList();
    }

    public static IMetadataTypeInfo GetFirstGenericArgument(this IMetadataField genericField)
    {
      var genericArgument = ((IMetadataClassType) genericField.Type).Arguments.First();
      return ((IMetadataClassType) genericArgument).Type;
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