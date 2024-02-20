using System;

namespace Machine.Specifications
{
    public static class ComparableExtensions
    {
        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw Exceptions.Specification("Should be greater than {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) <= 0)
            {
                throw Exceptions.Specification("Should be greater than {0} but is {1}", arg2, arg1);
            }
        }

        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw Exceptions.Specification("Should be greater than or equal to {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) < 0)
            {
                throw Exceptions.Specification("Should be greater than or equal to {0} but is {1}", arg2, arg1);
            }
        }

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw Exceptions.Specification("Should be less than {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) >= 0)
            {
                throw Exceptions.Specification("Should be less than {0} but is {1}", arg2, arg1);
            }
        }

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw Exceptions.Specification("Should be less than or equal to {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) > 0)
            {
                throw Exceptions.Specification("Should be less than or equal to {0} but is {1}", arg2, arg1);
            }
        }

        private static object TryToChangeType(this IComparable original, Type type)
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
