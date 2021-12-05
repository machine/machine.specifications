using System;
using System.Collections.Generic;
using System.Text;

namespace Machine.Specifications
{
    internal static class ComparableExtensions
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
    }
}
