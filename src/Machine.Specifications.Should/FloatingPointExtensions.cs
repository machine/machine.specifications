using System;
using Machine.Specifications.Formatting;

namespace Machine.Specifications
{
    public static class FloatingPointExtensions
    {
        public static void ShouldBeCloseTo(this float actual, float expected)
        {
            ShouldBeCloseTo(actual, expected, 0.0000001f);
        }

        public static void ShouldBeCloseTo(this float actual, float expected, float tolerance)
        {
            if (Math.Abs(actual - expected) > tolerance)
            {
                throw new SpecificationException($"Should be within {tolerance.Format()} of {expected.Format()} but is {actual.Format()}");
            }
        }

        public static void ShouldBeCloseTo(this double actual, double expected)
        {
            ShouldBeCloseTo(actual, expected, 0.0000001f);
        }

        public static void ShouldBeCloseTo(this double actual, double expected, double tolerance)
        {
            if (Math.Abs(actual - expected) > tolerance)
            {
                throw new SpecificationException($"Should be within {tolerance.Format()} of {expected.Format()} but is {actual.Format()}");
            }
        }

        public static void ShouldBeCloseTo(this decimal actual, decimal expected)
        {
            ShouldBeCloseTo(actual, expected, 0.0000001m);
        }

        public static void ShouldBeCloseTo(this decimal actual, decimal expected, decimal tolerance)
        {
            if (Math.Abs(actual - expected) > tolerance)
            {
                throw new SpecificationException($"Should be within {tolerance.Format()} of {expected.Format()} but is {actual.Format()}");
            }
        }
    }
}
