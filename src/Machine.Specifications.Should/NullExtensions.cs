﻿using JetBrains.Annotations;
using Machine.Specifications.Formatting;

namespace Machine.Specifications
{
    public static class NullExtensions
    {
        [AssertionMethod]
        public static void ShouldBeNull([AssertionCondition(AssertionConditionType.IS_NULL)] this object anObject)
        {
            if (anObject != null)
            {
                throw new SpecificationException($"Should be [null] but is {anObject.ToUsefulString()}");
            }
        }

        [AssertionMethod]
        public static void ShouldNotBeNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this object anObject)
        {
            if (anObject == null)
            {
                throw new SpecificationException("Should be [not null] but is [null]");
            }
        }
    }
}
