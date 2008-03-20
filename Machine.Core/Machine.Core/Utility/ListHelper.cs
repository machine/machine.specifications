using System;
using System.Collections.Generic;

namespace Machine.Core.Utility
{
  public static class ListHelper
  {
    public static TType Last<TType>(IList<TType> collection)
    {
      return collection[collection.Count - 1];
    }
  }
}
