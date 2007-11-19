using System.Collections.Generic;

using NUnit.Framework;

namespace ObjectFactories
{
  public static class EnumerableAssert
  {
    public static void IsEmpty<T>(IEnumerable<T> enumerable)
    {
      CollectionAssert.IsEmpty(new List<T>(enumerable));
    }
  }
}