using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{ 
    public class NoResult : ComparisionResult
    {
        public NoResult() : base()
        {
            this.FoundResult = false;
        }
    }
    public class ComparisionResult
    {
        public bool FoundResult { get; set; }
        public int Result { get; set; }

        public ComparisionResult(int result)
        {
            this.FoundResult = true;
            this.Result = result;
        }

        protected ComparisionResult()
        {
        }
    }
        
    public interface IComparerStrategy<T>
    {
        ComparisionResult Compare(T x, T y);
    }

    class DefaultComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            // Last case, rely on Object.Equals
            return object.Equals(x, y) ? 0 : -1;
        }
    }

    class ComparerStrategy<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            // Implements IEquatable<T>?
            var equatable = x as IEquatable<T>;

            if (equatable != null)
                return new ComparisionResult(equatable.Equals(y) ? 0 : -1);
            return new NoResult();
        }
    }

    class ComparableComparerStrategy<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            // Implements IComparable<T>?
            IComparable<T> comparable1 = x as IComparable<T>;

            if (comparable1 != null)
                return new ComparisionResult(comparable1.CompareTo(y)); 

            // Implements IComparable?
            IComparable comparable2 = x as IComparable;

            if (comparable2 != null)
                return new ComparisionResult(comparable2.CompareTo(y));

            return new NoResult();

        }
    }

    class TypeComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            if (x.GetType() != y.GetType())
                return new ComparisionResult(-1);
            return new NoResult();
        }
    }

    class GenericTypeComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            Type type = typeof(T);
            // Null?
            if (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
            {
                if (object.Equals(x, default(T)))
                {
                    return new ComparisionResult(object.Equals(y, default(T)) ? 0 : -1);
                }

                if (object.Equals(y, default(T)))
                    return new ComparisionResult(-1);
            }
            return new NoResult();
        }
    }

    class EnumerableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            // Enumerable?
            var enumerableX = x as IEnumerable;
            var enumerableY = y as IEnumerable;

            if (enumerableX != null && enumerableY != null)
            {
                IEnumerator enumeratorX = enumerableX.GetEnumerator();
                IEnumerator enumeratorY = enumerableY.GetEnumerator();

                while (true)
                {
                    bool hasNextX = enumeratorX.MoveNext();
                    bool hasNextY = enumeratorY.MoveNext();

                    if (!hasNextX || !hasNextY)
                        return new ComparisionResult(hasNextX == hasNextY ? 0 : -1);

                    if (!object.Equals(enumeratorX.Current, enumeratorY.Current))
                        return new ComparisionResult(-1);
                }
            }
            return new NoResult();
        }
    }

    public class ComparerFactory
    {
        
        public static IEnumerable<IComparerStrategy<T>> GetComparers<T>()
        {
            return new IComparerStrategy<T>[]
                   {
                       new EnumerableComparer<T>(),
                       new GenericTypeComparer<T>(),
                       new TypeComparer<T>()
                   };
        }

        public static IComparer<T> GetDefaultComparer<T>()
        {
            return new DefaultComparer<T>();
        }
    }

    


  // Borrowed from XUnit, licened under MS-PL
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
}