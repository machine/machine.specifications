using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Machine.Specifications
{
    internal static class BooleanExtensions
    {
        [AssertionMethod]
        public static void ShouldBeFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] this bool condition)
        {
            if (condition)
            {
                throw new SpecificationException("Should be [false] but is [true]");
            }
        }

        [AssertionMethod]
        public static void ShouldBeTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool condition)
        {
            if (!condition)
            {
                throw new SpecificationException("Should be [true] but is [false]");
            }
        }
    }
}
