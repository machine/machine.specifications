using System;
using System.Reflection;

namespace Machine.Specifications.Model
{
    public class Prerequesite 
    {
        public Prerequesite(Delegate condition, FieldInfo specificationField)
        {
            this.Condition = condition;
            this.SpecificationField = specificationField;
        }

        /// <summary>
        /// The condition to be met before we even execute the assertion.
        /// </summary>
        public Delegate Condition { get; set; }

        /// <summary>
        /// Typically, the <see cref="It"/> field containing the assertion.
        /// </summary>
        public FieldInfo SpecificationField { get; set; }
    }
}