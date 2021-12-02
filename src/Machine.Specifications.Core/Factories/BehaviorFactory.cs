using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Model;
using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
    internal class BehaviorFactory
    {
        private readonly SpecificationFactory specificationFactory = new SpecificationFactory();

        public Behavior CreateBehaviorFrom(FieldInfo behaviorField, Context context)
        {
            var behaviorType = behaviorField.FieldType.GetGenericArguments().First();

            EnsureBehaviorHasBehaviorsAttribute(behaviorType);
            EnsureBehaviorDoesNotHaveFrameworkFieldsExceptIt(behaviorType);

            var behaviorInstance = Activator.CreateInstance(behaviorType);

            EnsureAllBehaviorFieldsAreInContext(behaviorType, context);

            var isIgnored = behaviorField.HasAttribute(new IgnoreAttributeFullName()) ||
                            behaviorInstance.GetType().GetTypeInfo().HasAttribute(new IgnoreAttributeFullName());

            var behavior = new Behavior(behaviorField.FieldType, behaviorInstance, context, isIgnored);

            var itFieldInfos = behaviorType.GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName());

            CreateBehaviorSpecifications(itFieldInfos, behaviorField, behavior);

            return behavior;
        }

        private void EnsureBehaviorHasBehaviorsAttribute(Type behaviorType)
        {
            if (!behaviorType.GetTypeInfo().HasAttribute(new BehaviorAttributeFullName()))
            {
                throw new SpecificationUsageException(
                    $"Behaviors require the BehaviorsAttribute on the type containing the Specifications. Attribute is missing from {behaviorType.FullName}");
            }
        }

        private void EnsureBehaviorDoesNotHaveFrameworkFieldsExceptIt(Type behaviorType)
        {
            if (behaviorType.GetInstanceFieldsOfUsage(new SetupDelegateAttributeFullName()).Any())
            {
                throw new SpecificationUsageException($"You cannot have setup actions on Behaviors. Setup action found in {behaviorType.FullName}");
            }

            if (behaviorType.GetInstanceFieldsOfUsage(new ActDelegateAttributeFullName()).Any())
            {
                throw new SpecificationUsageException($"You cannot have act actions on Behaviors. Act action found in {behaviorType.FullName}");
            }

            if (behaviorType.GetInstanceFieldsOfUsage(new CleanupDelegateAttributeFullName()).Any())
            {
                throw new SpecificationUsageException($"You cannot have cleanup actions on Behaviors. Cleanup action found in {behaviorType.FullName}");
            }

            if (behaviorType.GetInstanceFieldsOfUsage(new BehaviorDelegateAttributeFullName()).Any())
            {
                throw new SpecificationUsageException($"You cannot nest behaviors. Nested behaviors found in {behaviorType.FullName}");
            }
        }

        private void EnsureAllBehaviorFieldsAreInContext(Type behaviorType, Context context)
        {
            var behaviorFieldsRequiredInContext = behaviorType.GetStaticProtectedOrInheritedFields()
                .Where(x => !x.IsLiteral);

            var contextFields = context.Instance.GetType()
                .GetStaticProtectedOrInheritedFields()
                .ToArray();

            foreach (var requiredField in behaviorFieldsRequiredInContext)
            {
                var requiredFieldName = requiredField.Name;
                var contextField = contextFields.SingleOrDefault(x => x.Name == requiredFieldName);

                EnsureContextFieldExists(context, contextField, requiredField, behaviorType);
                EnsureContextFieldIsCompatibleType(context, contextField, requiredField, behaviorType);
            }
        }

        private static void EnsureContextFieldExists(Context context, FieldInfo contextField, FieldInfo requiredField, Type behaviorType)
        {
            if (contextField != null)
            {
                return;
            }

            throw new SpecificationUsageException($"The context {context.Instance.GetType().FullName} behaves like {behaviorType.FullName} but does not define the behavior required field {requiredField.FieldType.FullName} {requiredField.Name}");
        }

        private static void EnsureContextFieldIsCompatibleType(Context context, FieldInfo contextField, FieldInfo requiredField, Type behaviorType)
        {
            if (requiredField.FieldType.IsAssignableFrom(contextField.FieldType))
            {
                return;
            }

            throw new SpecificationUsageException($"The context {context.Instance.GetType().FullName} behaves like {behaviorType.FullName} but defines the behavior field {requiredField.FieldType.FullName} {requiredField.Name} using an incompatible type.");
        }

        private void CreateBehaviorSpecifications(IEnumerable<FieldInfo> fieldInfos, FieldInfo behaviorField, Behavior behavior)
        {
            foreach (var fieldInfo in fieldInfos)
            {
                var specification = specificationFactory.CreateSpecificationFromBehavior(behavior, behaviorField, fieldInfo);
                behavior.AddSpecification(specification);
            }
        }
    }
}
