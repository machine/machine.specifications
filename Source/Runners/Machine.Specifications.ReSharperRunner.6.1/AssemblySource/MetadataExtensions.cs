namespace Machine.Specifications.ReSharperRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.Psi;

    using Machine.Specifications.Sdk;

    public static class MetadataExtensions
    {
        public static bool IsContext(this IMetadataTypeInfo type)
        {
            return !type.IsAbstract
                && !type.IsStruct()
                && !type.GenericParameters.Any()
                && !type.HasCustomAttribute(new BehaviorAttributeFullName())
                && (type.GetSpecifications().Any() || type.GetBehaviors().Any());
        }

        public static IEnumerable<IMetadataField> GetSpecifications(this IMetadataTypeInfo type)
        {
            var privateFieldsOfType = type.GetInstanceFieldsOfType(new ItDelegateFullName());
            return privateFieldsOfType;
        }

        public static IEnumerable<IMetadataField> GetBehaviors(this IMetadataTypeInfo type)
        {
            IEnumerable<IMetadataField> behaviorFields = type.GetInstanceFieldsOfType(new BehavesLikeDelegateFullName());
            foreach (IMetadataField field in behaviorFields)
            {
                if (field.GetFirstGenericArgument().HasCustomAttribute(new BehaviorAttributeFullName()))
                {
                    yield return field;
                }
            }
        }

        public static string GetSubjectString(this IMetadataEntity type)
        {
            var attributes = GetSubjectAttributes(type, new SubjectAttributeFullName());
            if (attributes.Count != 1)
            {
                var asMember = type as IMetadataTypeMember;
                if (asMember != null && asMember.DeclaringType != null)
                {
                    return asMember.DeclaringType.GetSubjectString();
                }

                return null;
            }

            IMetadataCustomAttribute attribute = attributes.First();
            string[] parameterNames = attribute.ConstructorArguments.Select(GetParameterName).ToArray();

            return string.Join(" ", parameterNames);
        }

        public static ICollection<string> GetTags(this IMetadataEntity type)
        {
            return
                type.AndAllBaseTypes()
                    .SelectMany(x => x.GetCustomAttributes(new TagsAttributeFullName()))
                    .Select(x => x.ConstructorArguments)
                    .Flatten(
                             tag => tag.FirstOrDefault().Value as string,
                        tag => tag.Skip(1).FirstOrDefault().ValuesArray.Select(v => v.Value as string))
                    .Distinct()
                    .ToList();
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
            return type.HasCustomAttribute(new IgnoreAttributeFullName());
        }

        public static string FullyQualifiedName(this IMetadataClassType classType)
        {
            return FullyQualifiedName(classType, false);
        }

        private static string GetParameterName(MetadataAttributeValue x)
        {
            var typeArgument = x.Value as IMetadataClassType;
            if (typeArgument != null)
            {
                return new ClrTypeName(typeArgument.Type.FullyQualifiedName).ShortName;
            }

            return (string)x.Value;
        }

        private static bool IsStruct(this IMetadataTypeInfo type)
        {
            if (type.Base != null)
            {
                return type.Base.Type.FullyQualifiedName == typeof(ValueType).FullName;
            }

            return false;
        }

        private static IEnumerable<IMetadataField> GetInstanceFieldsOfType(
            this IMetadataTypeInfo type,
            string fullyQualifiedName)
        {
            var metadataFields = type.GetInstanceFields();
            var fields = metadataFields.Where(x => x.Type is IMetadataClassType);
            return fields.Where(x => (((IMetadataClassType)x.Type).Type.FullyQualifiedName == fullyQualifiedName));
        }

        private static IEnumerable<IMetadataField> GetInstanceFields(this IMetadataTypeInfo type)
        {
            return type.GetFields().Where(field => !field.IsStatic);
        }

        private static IList<IMetadataCustomAttribute> GetSubjectAttributes(
            IMetadataEntity type,
            SubjectAttributeFullName subjectAttributeFullName)
        {
            return type.CustomAttributes.Where(
                attribute => attribute.AndAllBaseTypes()
                    .Any(i => i.FullyQualifiedName == subjectAttributeFullName)).ToList();
        }

        private static IEnumerable<IMetadataTypeInfo> AndAllBaseTypes(this IMetadataEntity type)
        {
            Func<IMetadataEntity, IMetadataTypeInfo> getTypeFromAttributeConstructor = entity =>
            {
                var attribute = type as IMetadataCustomAttribute;

                return attribute != null ? attribute.UsedConstructor.DeclaringType : null;
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

            while (typeInfo.Base != null)
            {
                yield return typeInfo.Base.Type;

                typeInfo = typeInfo.Base.Type;
            }
        }

        private static IEnumerable<TResult> Flatten<TSource, TResult>(
            this IEnumerable<TSource> sources,
            Func<TSource, TResult> singleResultSelector,
            Func<TSource, IEnumerable<TResult>> collectionResultSelector)
        {
            foreach (var source in sources)
            {
                yield return singleResultSelector(source);
                
                IEnumerable<TResult> resultSelector = collectionResultSelector(source);
                if (resultSelector == null)
                {
                    yield break;
                }

                foreach (var coll in collectionResultSelector(source))
                {
                    yield return coll;
                }
            }
        }

        private static string FullyQualifiedName(this IMetadataClassType classType, bool appendAssembly)
        {
            var fullyQualifiedName = new StringBuilder();

            fullyQualifiedName.Append(classType.Type.FullyQualifiedName);

            if (classType.Arguments.Length > 0)
            {
                fullyQualifiedName.Append("[");
                string[] fullQualifiedNames =
                    classType.Arguments.Select(x => x as IMetadataClassType)
                        .Where(x => x != null)
                        .Select(x => "[" + x.FullyQualifiedName(true) + "]")
                        .ToArray();

                fullyQualifiedName.Append(string.Join(",", fullQualifiedNames));
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