using System;
using Machine.Specifications.Formatting;

namespace Machine.Specifications
{
    public static class DateTimeExtensions
    {
        public static void ShouldBeCloseTo(this TimeSpan actual, TimeSpan expected, TimeSpan tolerance)
        {
            if (Math.Abs(actual.Ticks - expected.Ticks) > tolerance.Ticks)
            {
                throw new SpecificationException($"Should be within {tolerance.ToUsefulString()} of {expected.ToUsefulString()} but is {actual.ToUsefulString()}");
            }
        }

        public static void ShouldBeCloseTo(this DateTime actual, DateTime expected, TimeSpan tolerance)
        {
            var difference = expected - actual;

            if (Math.Abs(difference.Ticks) > tolerance.Ticks)
            {
                throw new SpecificationException($"Should be within {tolerance.ToUsefulString()} of {expected.ToUsefulString()} but is {actual.ToUsefulString()}");
            }
        }
    }
}
