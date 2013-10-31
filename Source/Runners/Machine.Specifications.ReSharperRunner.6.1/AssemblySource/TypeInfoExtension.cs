using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Metadata.Reader.API;

using Machine.Specifications.Sdk;

namespace Machine.Specifications.ReSharperRunner.AssemblySource
{
    internal static class TypeInfoExtension
    {



        public static ICollection<string> GetTags(this IMetadataEntity type)
        {
            return type.AndAllBaseTypes()
                    .SelectMany(x => x.GetCustomAttributes(new TagsAttributeFullName()))
                    .Select(x => x.ConstructorArguments)
                    .Flatten(tag => tag.FirstOrDefault().Value as string,
                    tag => tag.Skip(1).FirstOrDefault().ValuesArray.Select(v => v.Value as string))
                    .Distinct()
                    .ToList();
        }

        static IEnumerable<IMetadataTypeInfo> AndAllBaseTypes(this IMetadataEntity type)
        {
            Func<IMetadataEntity, IMetadataTypeInfo> getTypeFromAttributeConstructor = entity =>
            {
                var attr = type as IMetadataCustomAttribute;
                if (attr == null)
                {
                    return null;
                }

                return attr.UsedConstructor.DeclaringType;
            };

            var typeInfo = type as IMetadataTypeInfo;
            if (typeInfo == null)
            {
                // type might be an attribute - which cannot be cast as IMetadataTypeInfo.
                typeInfo = getTypeFromAttributeConstructor(type);
                if (typeInfo == null)
                {
                    // No idea how the get the type of the IMetadataEntity.
                    yield break;
                }
            }
            yield return typeInfo;
            while (typeInfo.Base != null && typeInfo.Base.Type != null)
            {
                yield return typeInfo.Base.Type;
                typeInfo = typeInfo.Base.Type;
            }
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


    }
}
