using System;
using System.Linq;
using System.Linq.Expressions;
using Machine.Specifications.Text;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    public static class ShouldExtensionMethods
    {
        private static bool SafeEquals<T>(this T left, T right)
        {
            var comparer = new AssertComparer<T>();

            return comparer.Compare(left, right) == 0;
        }

        public static T ShouldEqual<T>(this T actual, T expected)
        {
            if (!actual.SafeEquals(expected))
            {
                throw new SpecificationException(PrettyPrintingExtensions.FormatErrorMessage(actual, expected));
            }

            return actual;
        }

        public static object ShouldNotEqual<T>(this T actual, T expected)
        {
            if (actual.SafeEquals(expected))
            {
                throw new SpecificationException($"Should not equal {expected.ToUsefulString()} but does: {actual.ToUsefulString()}");
            }

            return actual;
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

        private static SpecificationException NewException(string message, params object[] parameters)
        {
            if (parameters.Any())
            {
                return new SpecificationException(string.Format(message.EnsureSafeFormat(), parameters.Select(x => x.ToUsefulString()).Cast<object>().ToArray()));
            }

            return new SpecificationException(message);
        }
    }
}
