using System;
using System.Collections.Generic;
using System.Text;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    internal static class DateTimeExtensions
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
