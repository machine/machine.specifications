using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;
using Machine.Specifications.Utility.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Factories
{
    public class SpecificationFactory
    {
        public Specification CreateSpecification(Context context, FieldInfo specificationField, IList<FieldInfo> prerequisiteFields)
        {
            bool isIgnored = context.IsIgnored || specificationField.HasAttribute(new IgnoreAttributeFullName());
            var it = (Delegate)specificationField.GetValue(context.Instance);
            Prerequesite[] prerequisites = prerequisiteFields.Select(o => new Prerequesite((Delegate)o.GetValue(context.Instance), o)).ToArray();
            string name = specificationField.Name.ToFormat();

            return new Specification(name, specificationField.FieldType, it, isIgnored, specificationField, prerequisites);
        }

        public Specification CreateSpecificationFromBehavior(Behavior behavior, FieldInfo specificationField)
        {
            bool isIgnored = behavior.IsIgnored || specificationField.HasAttribute(new IgnoreAttributeFullName());
            var it = (Delegate)specificationField.GetValue(behavior.Instance);
            string name = specificationField.Name.ToFormat();

            return new BehaviorSpecification(name, specificationField.FieldType, it, isIgnored, specificationField, behavior.Context, behavior);
        }
    }
}
