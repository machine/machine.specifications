using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Model;
using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
    public class ContextFactory
    {
        private readonly BehaviorFactory behaviorFactory = new BehaviorFactory();

        private readonly SpecificationFactory specificationFactory = new SpecificationFactory();

        private static int allowedNumberOfBecauseBlocks = 1;

        public Context CreateContextFrom(object instance, FieldInfo fieldInfo)
        {
            if (fieldInfo.IsOfUsage(new AssertDelegateAttributeFullName()))
            {
                return CreateContextFrom(instance, new[] {fieldInfo});
            }

            return CreateContextFrom(instance);
        }

        public Context CreateContextFrom(object instance)
        {
            var type = instance.GetType();
            var fieldInfos = type.GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName(), new BehaviorDelegateAttributeFullName());

            return CreateContextFrom(instance, fieldInfos);
        }

        private Context CreateContextFrom(object instance, IEnumerable<FieldInfo> acceptedSpecificationFields)
        {
            var type = instance.GetType();
            var fieldInfos = type.GetInstanceFields();
            var itFieldInfos = new List<FieldInfo>();
            var itShouldBehaveLikeFieldInfos = new List<FieldInfo>();

            var contextClauses = ExtractPrivateFieldValues(instance, true, new SetupDelegateAttributeFullName());
            contextClauses = contextClauses.Reverse();

            var cleanupClauses = ExtractPrivateFieldValues(instance, true, new CleanupDelegateAttributeFullName());

            var becauses = ExtractPrivateFieldValues(instance, false, new ActDelegateAttributeFullName())
                .Reverse()
                .ToArray();

            var acceptedSpecFields = acceptedSpecificationFields.ToArray();

            if (becauses.Length > allowedNumberOfBecauseBlocks)
            {
                throw new SpecificationUsageException($"There can only be one Because clause. Found {becauses.Length} Becauses in the type hierarchy of {instance.GetType().FullName}.");
            }

            var concern = ExtractSubject(type);
            var isSetupForEachSpec = IsSetupForEachSpec(type);

            var isIgnored = type.GetTypeInfo().HasAttribute(new IgnoreAttributeFullName());
            var tags = ExtractTags(type);

            var context = new Context(type,
                instance,
                contextClauses,
                becauses,
                cleanupClauses,
                concern,
                isIgnored,
                tags,
                isSetupForEachSpec);

            foreach (var info in fieldInfos)
            {
                if (acceptedSpecFields.Contains(info) && info.IsOfUsage(new AssertDelegateAttributeFullName()))
                {
                    itFieldInfos.Add(info);
                }

                if (acceptedSpecFields.Contains(info) && info.IsOfUsage(new BehaviorDelegateAttributeFullName()))
                {
                    itShouldBehaveLikeFieldInfos.Add(info);
                }
            }

            CreateSpecifications(itFieldInfos, context);
            CreateSpecificationsFromBehaviors(itShouldBehaveLikeFieldInfos, context);

            return context;
        }

        private static IEnumerable<Tag> ExtractTags(Type type)
        {
            var extractor = new AttributeTagExtractor();

            return extractor.ExtractTags(type);
        }

        private static bool IsSetupForEachSpec(Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes(typeof(SetupForEachSpecification), true).Any();
        }

        private static Subject ExtractSubject(Type type)
        {
            var attributes = type.GetTypeInfo().GetCustomAttributes(typeof(SubjectAttribute), true);

            if (attributes.Length > 1)
            {
                throw new SpecificationUsageException("Cannot have more than one Subject on a Context");
            }

            if (!attributes.Any())
            {
                if (type.DeclaringType == null)
                {
                    return null;
                }

                return ExtractSubject(type.DeclaringType);
            }

            var attribute = (SubjectAttribute) attributes.ElementAt(0);

            return attribute.CreateSubject();
        }

        private void CreateSpecifications(IEnumerable<FieldInfo> fieldInfos, Context context)
        {
            foreach (var fieldInfo in fieldInfos)
            {
                var specification = specificationFactory.CreateSpecification(context, fieldInfo);
                context.AddSpecification(specification);
            }
        }

        private void CreateSpecificationsFromBehaviors(IEnumerable<FieldInfo> itShouldBehaveLikeFieldInfos, Context context)
        {
            foreach (var itShouldBehaveLikeFieldInfo in itShouldBehaveLikeFieldInfos)
            {
                var behavior = behaviorFactory.CreateBehaviorFrom(itShouldBehaveLikeFieldInfo, context);

                foreach (var specification in behavior.Specifications)
                {
                    context.AddSpecification(specification);
                }
            }
        }

        private static void CollectFieldDetails<T>(FieldInspectionArguments<T> inspection)
        {
            if (inspection.CannotProceed)
            {
                return;
            }

            inspection.CollectFieldValue();

            CollectFieldDetails(inspection.DetailsForBaseType());

            if (inspection.IsNested)
            {
                CollectFieldDetails(inspection.DetailsForDeclaringType());
            }
        }

        private static IEnumerable<Delegate> ExtractPrivateFieldValues(object instance, bool ensureMaximumOfOne, AttributeFullName attributeFullName)
        {
            var details = FieldInspectionArguments<Delegate>.CreateFromInstance(instance, ensureMaximumOfOne, attributeFullName);

            CollectFieldDetails(details);

            return details.Items;
        }

        public static void ChangeAllowedNumberOfBecauseBlocksTo(int newValue)
        {
            allowedNumberOfBecauseBlocks = newValue;
        }
    }
}
