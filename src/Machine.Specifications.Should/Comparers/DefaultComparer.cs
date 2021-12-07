using System.Collections.Generic;

namespace Machine.Specifications.Comparers
{
    internal class DefaultComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
