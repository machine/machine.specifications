using System;
using System.Collections.Generic;
using System.Threading;

namespace Machine.Core.ValueTypes
{
  public class ValueTypeHelper
  {
    private static readonly ObjectEqualityFunctionFactory _objectEqualityFunctionFactory = new ObjectEqualityFunctionFactory();
    private static readonly CalculateHashCodeFunctionFactory _calculateHashCodeFunctionFactory = new CalculateHashCodeFunctionFactory();
    private static readonly ToStringFunctionFactory _toStringFunctionFactory = new ToStringFunctionFactory();
    private static readonly Dictionary<Type, CacheEntry> _cache = new Dictionary<Type, CacheEntry>();
    private static readonly ReaderWriterLock _lock = new ReaderWriterLock();

    private class CacheEntry
    {
      public ObjectEqualityFunction AreEqual;
      public CalculateHashCodeFunction CalculateHashCode;
      public ToStringFunction ToString;
    }

    public static bool AreEqual<TType>(TType a, TType b)
    {
      return Lookup(typeof(TType)).AreEqual(a, b);
    }

    public static bool AreEqual<TType>(object a, object b)
    {
      if (a is TType && b is TType)
      {
        return Lookup(typeof(TType)).AreEqual(a, b);
      }
      return false;
    }

    public static bool AreEqual(object a, object b)
    {
      throw new InvalidOperationException("You must use generic version of AreEqual!");
    }

    public static Int32 CalculateHashCode<TType>(TType a)
    {
      return Lookup(typeof(TType)).CalculateHashCode(a);
    }

    public static string ToString<TType>(TType a)
    {
      return Lookup(typeof(TType)).ToString(a);
    }

    private static CacheEntry Lookup(Type type)
    {
      CacheEntry entry;
      _lock.AcquireReaderLock(Timeout.Infinite);
      if (_cache.ContainsKey(type))
      {
        entry = _cache[type];
      }
      else
      {
        _lock.UpgradeToWriterLock(Timeout.Infinite);
        entry = _cache[type] = new CacheEntry();
        _cache[type].AreEqual = _objectEqualityFunctionFactory.CreateObjectEqualityFunction(type);
        _cache[type].CalculateHashCode = _calculateHashCodeFunctionFactory.CreateCalculateHashCodeFunction(type);
        _cache[type].ToString = _toStringFunctionFactory.CreateToStringFunction(type);
      }
      _lock.ReleaseLock();
      return entry;
    }
  }
}
