using System;
using System.Linq;
using Machine.Specifications.Text;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    public static class ComparableExtensions
    {
        public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be greater than {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) <= 0)
            {
                throw NewException("Should be greater than {0} but is {1}", arg2, arg1);
            }

            return arg1;
        }

        public static IComparable ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be greater than or equal to {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) < 0)
            {
                throw NewException("Should be greater than or equal to {0} but is {1}", arg2, arg1);
            }

            return arg1;
        }

        public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be less than {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) >= 0)
            {
                throw NewException("Should be less than {0} but is {1}", arg2, arg1);
            }

            return arg1;
        }

        public static IComparable ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be less than or equal to {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) > 0)
            {
                throw NewException("Should be less than or equal to {0} but is {1}", arg2, arg1);
            }

            return arg1;
        }

        private static object TryToChangeType(this object original, Type type)
        {
            try
            {
                return Convert.ChangeType(original, type);
            }
            catch
            {
                return original;
            }
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
