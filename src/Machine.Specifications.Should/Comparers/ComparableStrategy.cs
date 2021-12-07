using System;

namespace Machine.Specifications.Comparers
{
    internal class ComparableStrategy<T> : IEqualityStrategy<T>
    {
        public bool? Equals(T? x, T? y)
        {
            if (x is IComparable<T> genericComparable)
            {
                try
                {
                    return genericComparable.CompareTo(y!) == 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            if (x is IComparable comparable)
            {
                try
                {
                    return comparable.CompareTo(y!) == 0;
                }
                catch
                {
                }
            }

            return null;
        }
    }
}
