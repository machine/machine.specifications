using System;
using System.Reflection;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Model
{
    public class BehaviorSpecification : Specification
    {
        private readonly object behaviorInstance;

        private readonly object contextInstance;

        private readonly ConventionMapper mapper = new ConventionMapper();

        public BehaviorSpecification(
            string name,
            Type fieldType,
            FieldInfo behaviorField,
            Delegate it,
            bool isIgnored,
            FieldInfo fieldInfo,
            Context context,
            Behavior behavior)
            : base(name, fieldType, it, isIgnored, fieldInfo)
        {
            contextInstance = context.Instance;
            behaviorInstance = behavior.Instance;

            BehaviorFieldInfo = behaviorField;
        }

        public FieldInfo BehaviorFieldInfo { get; }

        protected override void InvokeSpecificationField()
        {
            mapper.MapPropertiesOf(contextInstance).To(behaviorInstance);

            base.InvokeSpecificationField();
        }

        private class ConventionMapper
        {
            internal ConventionMap MapPropertiesOf(object source)
            {
                return new ConventionMap(source);
            }

            internal class ConventionMap
            {
                private readonly object source;

                public ConventionMap(object source)
                {
                    this.source = source;
                }

                public void To(object target)
                {
                    var sourceFields = source.GetType().GetStaticProtectedOrInheritedFields();

                    foreach (var sourceField in sourceFields)
                    {
                        var targetField = target.GetType().GetStaticProtectedOrInheritedFieldNamed(sourceField.Name);

                        if (targetField == null)
                        {
                            continue;
                        }

                        targetField.SetValue(target, sourceField.GetValue(source));
                    }
                }
            }
        }
    }
}
