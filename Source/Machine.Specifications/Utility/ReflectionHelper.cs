using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Sdk;

namespace Machine.Specifications.Utility
{
    public static class ReflectionHelper
    {
        public static IEnumerable<FieldInfo> GetInstanceFields(this Type type)
        {                        
            var specificationFields = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);

            var prerequisiteFields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(IsPrerequisiteField);

            var consideredFields = specificationFields.Concat(prerequisiteFields);             

            return consideredFields;
        }

        static bool IsPrerequisiteField(FieldInfo field)
        {
            return field.FieldType == typeof(Requires);
        }

        public static IEnumerable<FieldInfo> GetStaticProtectedOrInheritedFields(this Type type)
        {
            return type
              .GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
              .Where(x => !x.IsPrivate);
        }

        public static IEnumerable<FieldInfo> GetInstanceFieldsOfUsage(this Type type, AttributeFullName attributeFullName)
        {
            return GetInstanceFields(type).Where(x => GetDelegateUsage(x) == attributeFullName);
        }

        public static IEnumerable<FieldInfo> GetInstanceFieldsOfUsage(this Type type, params AttributeFullName[] attributeFullNames)
        {
            if (attributeFullNames == null || attributeFullNames.Length == 0)
            {
                throw new ArgumentNullException("attributeFullNames");
            }

            var fields = GetInstanceFields(type);
            return attributeFullNames.Aggregate(Enumerable.Empty<FieldInfo>(),
                                    (current, usage) => current.Union(fields.Where(x => GetDelegateUsage(x) == usage)));
        }

        public static FieldInfo GetStaticProtectedOrInheritedFieldNamed(this Type type, string fieldName)
        {
            return type.GetStaticProtectedOrInheritedFields().SingleOrDefault(x => x.Name == fieldName);
        }

        public static bool IsOfUsage(this FieldInfo fieldInfo, AttributeFullName attributeFullName)
        {
            return GetDelegateUsage(fieldInfo) == attributeFullName;
        }

        static AttributeFullName GetDelegateUsage(this FieldInfo fieldInfo)
        {
            var fieldType = fieldInfo.FieldType;

            var attributeFullName = fieldType.GetCustomDelegateAttributeFullName();
            if (attributeFullName == null)
            {
                return null;
            }

            var invoke = fieldType.GetMethod("Invoke");

            // Do some validation to prevent messages with no indication of the cause of the problem later on.
            if (invoke == null)
            {
                throw new InvalidOperationException(string.Format("The delegate type {0} does not have an invoke method.",
                                                                  fieldType));
            }
            if (invoke.GetParameters().Length != 0)
            {
                throw new InvalidOperationException(string.Format("{0} delegates require 0 parameters, {1} has {2}.",
                                                                  attributeFullName,
                                                                  fieldType,
                                                                  invoke.GetParameters().Length));
            }

            if (attributeFullName is BehaviorAttributeFullName)
            {
                if (!fieldType.GetTypeInfo().IsGenericType)
                {
                    throw new InvalidOperationException(
                      string.Format("{0} delegates need to be generic types with 1 parameter. {1} is not a generic type.",
                                    attributeFullName,
                                    fieldType));
                }

                if (fieldType.GetGenericArguments().Length != 1)
                {
                    throw new InvalidOperationException(
                      string.Format("{0} delegates need to be generic types with 1 parameter. {1} has {2} parameters.",
                                    attributeFullName,
                                    fieldType,
                                    fieldType.GetTypeInfo().GetGenericParameterConstraints().Length));
                }
            }

            return attributeFullName;
        }
    }
}