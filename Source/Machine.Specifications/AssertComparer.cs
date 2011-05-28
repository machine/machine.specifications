﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
    class AssertComparer<T> : IComparer<T>, IEqualityComparer<T>
    {
        public int Compare(T x,
                           T y)
        {
 
            var comparers = ComparerFactory.GetComparers<T>();

            foreach (var comparer in comparers)
            {
                var comparisionResult = comparer.Compare(x,y);
                if (comparisionResult.FoundResult)
                    return comparisionResult.Result;
            }

            return ComparerFactory.GetDefaultComparer<T>().Compare(x, y);

        }

        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    // Borrowed from XUnit, licened under MS-PL
}