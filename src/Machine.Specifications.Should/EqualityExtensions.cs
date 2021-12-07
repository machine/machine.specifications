using System;
using System.Linq.Expressions;
using Machine.Specifications.Comparers;
using Machine.Specifications.Formatting;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    public static class EqualityExtensions
    {
        public static void ShouldEqual<T>(this T actual, T expected)
        {
            if (!AssertEqualityComparer<T>.Default.Equals(actual, expected))
            {
                throw new SpecificationException(PrettyPrintingExtensions.FormatErrorMessage(actual, expected));
            }
        }

        public static void ShouldNotEqual<T>(this T actual, T expected)
        {
            if (AssertEqualityComparer<T>.Default.Equals(actual, expected))
            {
                throw new SpecificationException($"Should not equal {expected.ToUsefulString()} but does: {actual.ToUsefulString()}");
            }
        }

        public static void ShouldMatch<T>(this T actual, Expression<Func<T, bool>> condition)
        {
            var matches = condition.Compile().Invoke(actual);

            if (matches)
            {
                return;
            }

            throw new SpecificationException($"Should match expression [{condition}], but does not.");
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            if (!ReferenceEquals(actual, expected))
            {
                throw new SpecificationException($"Should be the same as {expected} but is {actual}");
            }
        }

        public static void ShouldNotBeTheSameAs(this object actual, object expected)
        {
            if (ReferenceEquals(actual, expected))
            {
                throw new SpecificationException($"Should not be the same as {expected} but is {actual}");
            }
        }
    }
}
