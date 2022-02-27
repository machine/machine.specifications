using System;

namespace Machine.Specifications
{
    public static class TypeExtensions
    {
        public static void ShouldBeOfExactType<T>(this object? actual)
        {
            actual.ShouldBeOfExactType(typeof(T));
        }

        public static void ShouldBeOfExactType(this object? actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException($"Should be of type {expected} but is [null]");
            }

            if (actual.GetType() != expected)
            {
                throw new SpecificationException($"Should be of type {expected} but is of type {actual.GetType()}");
            }
        }

        public static void ShouldNotBeOfExactType<T>(this object? actual)
        {
            actual?.ShouldNotBeOfExactType(typeof(T));
        }

        public static void ShouldNotBeOfExactType(this object? actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException($"Should not be of type {expected} but is [null]");
            }

            if (actual.GetType() == expected)
            {
                throw new SpecificationException($"Should not be of type {expected} but is of type {actual.GetType()}");
            }
        }

        public static void ShouldBeAssignableTo<T>(this object? actual)
        {
            actual?.ShouldBeAssignableTo(typeof(T));
        }

        public static void ShouldBeAssignableTo(this object? actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException($"Should be assignable to type {expected} but is [null]");
            }

            if (!expected.IsInstanceOfType(actual))
            {
                throw new SpecificationException($"Should be assignable to type {expected} but is not. Actual type is {actual.GetType()}");
            }
        }

        public static void ShouldNotBeAssignableTo<T>(this object? actual)
        {
            actual?.ShouldNotBeAssignableTo(typeof(T));
        }

        public static void ShouldNotBeAssignableTo(this object? actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException($"Should not be assignable to type {expected} but is [null]");
            }

            if (expected.IsInstanceOfType(actual))
            {
                throw new SpecificationException($"Should not be assignable to type {expected} but is. Actual type is {actual.GetType()}");
            }
        }
    }
}
