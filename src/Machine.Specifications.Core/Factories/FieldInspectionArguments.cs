using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
    public class FieldInspectionArguments<T> : IInspectFields
    {
        private readonly Type target;

        private readonly Func<object> instanceResolver;

        private readonly bool ensureMaximumOfOne;

        private readonly AttributeFullName attributeFullName;

        private readonly object instance;

        public ICollection<T> Items { get; }

        public FieldInspectionArguments(
            Type target,
            Func<object> instanceResolver,
            ICollection<T> items,
            bool ensureMaximumOfOne,
            AttributeFullName attributeFullName)
        {
            this.target = target;
            this.instanceResolver = instanceResolver;
            this.ensureMaximumOfOne = ensureMaximumOfOne;
            this.attributeFullName = attributeFullName;

            instance = instanceResolver();

            Items = items;
        }

        public bool IsAbstractOrSealed => target.GetTypeInfo().IsAbstract && target.GetTypeInfo().IsSealed;

        private Type DeclaringType => TargetType.DeclaringType;

        private Type TargetType => instance.GetType();

        public bool IsNested => TargetType.IsNested;

        private bool DeclaringTypeIsObject => DeclaringType == typeof(object);

        private bool DeclaringTypeIsNotGeneric => !DeclaringType.GetTypeInfo().ContainsGenericParameters;

        private int NumberOfGenericParametersOnDeclaringType => DeclaringType.GetTypeInfo().GetGenericTypeDefinition().GetGenericArguments().Length;

        public bool CannotProceed => target == typeof(object);

        public void CollectFieldValue()
        {
            var fields = target.GetInstanceFieldsOfUsage(attributeFullName)
                .ToArray();

            if (ensureMaximumOfOne && fields.Length > 1)
            {
                throw new SpecificationUsageException($"You cannot have more than one {fields.First().FieldType.Name} clause in {target.FullName}");
            }

            var field = fields.FirstOrDefault();

            if (field != null)
            {
                var val = (T) field.GetValue(instance);

                Items.Add(val);
            }
        }

        public FieldInspectionArguments<T> DetailsForBaseType()
        {
            return new FieldInspectionArguments<T>(
                target.GetTypeInfo().BaseType,
                instanceResolver,
                Items,
                ensureMaximumOfOne,
                attributeFullName);
        }

        public FieldInspectionArguments<T> DetailsForDeclaringType()
        {
            var declaringType = GetFullyClosedDeclaringType();
            var factory = GetNextInstanceResolver(declaringType);

            return new FieldInspectionArguments<T>(declaringType,
                factory,
                Items,
                ensureMaximumOfOne,
                attributeFullName);
        }

        private static Func<object> GetNextInstanceResolver(Type declaringType)
        {
            var typeInfo = declaringType.GetTypeInfo();

            if (typeInfo.IsAbstract || typeInfo.IsSealed)
            {
                return () => declaringType;
            }

            return () => Activator.CreateInstance(declaringType);
        }

        public static FieldInspectionArguments<T> CreateFromInstance(object instance, bool ensureMaximumOfOne, AttributeFullName attributeFullName)
        {
            var delegates = new List<T>();
            var type = instance.GetType();

            return new FieldInspectionArguments<T>(
                type,
                () => instance,
                delegates,
                ensureMaximumOfOne,
                attributeFullName);
        }

        private Type GetFullyClosedDeclaringType()
        {
            return instance == null
                ? target.DeclaringType
                : GetDeclaringType();
        }

        private Type MakeClosedVersionOfDeclaringType()
        {
            var parameters = GetGenericParametersForDeclaringType();

            return DeclaringType.MakeGenericType(parameters);
        }

        private Type GetDeclaringType()
        {
            if (DeclaringTypeIsObject || DeclaringTypeIsNotGeneric)
            {
                return DeclaringType;
            }

            return MakeClosedVersionOfDeclaringType();
        }

        private Type[] GetGenericParametersForDeclaringType()
        {
            var parameters = TargetType.GetGenericArguments()
                .Take(NumberOfGenericParametersOnDeclaringType);

            return parameters.ToArray();
        }
    }
}
