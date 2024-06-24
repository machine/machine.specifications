using System;

namespace Machine.Specifications.Comparers
{
    internal class EquatableStrategy<T> : IEqualityStrategy<T>
    {
        public bool? Equals(T? x, T? y)
        {
            if (x is not IEquatable<T> equatable)
            {
                return null;
            }

            if (y == null)
            {
                return false;
            }

            try
            {
                return equatable.Equals(y);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
