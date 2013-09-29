﻿using System.Collections.Generic;

namespace Machine.Specifications.ComparerStrategies
{
  class ComparerFactory
  {
    public static IEnumerable<IComparerStrategy<T>> GetComparers<T>()
    {
      return new IComparerStrategy<T>[]
             {
               new EnumerableComparer<T>(),
               new GenericTypeComparer<T>(),
               new TypeComparer<T>(),
               new ComparableComparer<T>(),
               new EquatableComparer<T>()
             };
    }

    public static IComparer<T> GetDefaultComparer<T>()
    {
      return new DefaultComparer<T>();
    }
  }
}