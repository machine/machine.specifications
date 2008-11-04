using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using NUnit.Framework;

namespace Machine.Specifications
{
  public static class CollectionExtensionMethods
  {
    public static ArrayList ToArrayList(this IEnumerable enumerable)
    {
      ArrayList arrayList = new ArrayList();
      foreach (object obj in enumerable)
      {
        arrayList.Add(obj);
      }
      return arrayList;
    }
    
    public static bool ContainsAny<T>(this IEnumerable<T> collection, IEnumerable<T> values)
    {
      foreach (T item in values)
      {
        if (collection.Contains(item))
        {
          return true;
        }
      }
      return false;
    }
  }
  public static class ShouldExtensionMethods
  {
    public static void ShouldBeFalse(this bool condition)
    {
      Assert.IsFalse(condition);
    }

    public static void ShouldBeTrue(this bool condition)
    {
      Assert.IsTrue(condition);
    }

    public static object ShouldEqual(this object actual, object expected)
    {
      Assert.AreEqual(expected, actual);
      return expected;
    }

    public static object ShouldNotEqual(this object actual, object expected)
    {
      Assert.AreNotEqual(expected, actual);
      return expected;
    }

    public static void ShouldBeNull(this object anObject)
    {
      Assert.IsNull(anObject);
    }

    public static void ShouldNotBeNull(this object anObject)
    {
      Assert.IsNotNull(anObject);
    }

    public static object ShouldBeTheSameAs(this object actual, object expected)
    {
      Assert.AreSame(expected, actual);
      return expected;
    }

    public static object ShouldNotBeTheSameAs(this object actual, object expected)
    {
      Assert.AreNotSame(expected, actual);
      return expected;
    }

    public static void ShouldBeOfType(this object actual, Type expected)
    {
      Assert.IsInstanceOfType(expected, actual);
    }

    public static void ShouldBeOfType<T>(this object actual)
    {
      Assert.IsInstanceOfType(typeof(T), actual);
    }

    public static void ShouldBe(this object actual, Type expected)
    {
      Assert.IsInstanceOfType(expected, actual);
    }

    public static void ShouldNotBeOfType(this object actual, Type expected)
    {
      Assert.IsNotInstanceOfType(expected, actual);
    }

    public static void ShouldContain(this IList actual, params object[] expected)
    {
      foreach (var item in expected)
      {
        Assert.Contains(item, actual);
      }
    }

    public static void ShouldContain<T>(this IEnumerable<T> actual, params T[] expected)
    {
      var actualList = new List<T>(actual);
      foreach (var item in expected)
      {
        Assert.Contains(item, actualList);
      }
    }

    public static void ShouldNotContain(this IEnumerable collection, object expected)
    {
      CollectionAssert.DoesNotContain(collection, expected);
    }

    public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
    {
      Assert.Greater(arg1, arg2);
      return arg2;
    }

    public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
    {
      Assert.Less(arg1, arg2);
      return arg2;
    }

    public static void ShouldBeEmpty(this IEnumerable collection)
    {
      Assert.IsEmpty(collection.ToArrayList());
    }

    public static void ShouldBeEmpty(this string aString)
    {
      Assert.IsEmpty(aString);
    }

    public static void ShouldNotBeEmpty(this IEnumerable collection)
    {
      Assert.IsNotEmpty(collection.ToArrayList());
    }

    public static void ShouldNotBeEmpty(this string aString)
    {
      Assert.IsNotEmpty(aString);
    }

    public static void ShouldContain(this string actual, string expected)
    {
      StringAssert.Contains(expected, actual);
    }

    public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
    {
      StringAssert.AreEqualIgnoringCase(expected, actual);
      return expected;
    }

    public static void ShouldStartWith(this string actual, string expected)
    {
      StringAssert.StartsWith(expected, actual);
    }

    public static void ShouldEndWith(this string actual, string expected)
    {
      StringAssert.EndsWith(expected, actual);
    }

    public static void ShouldBeSurroundedWith(this string actual, string expectedStartDelimiter,
      string expectedEndDelimiter)
    {
      StringAssert.StartsWith(expectedStartDelimiter, actual);
      StringAssert.EndsWith(expectedEndDelimiter, actual);
    }

    public static void ShouldBeSurroundedWith(this string actual, string expectedDelimiter)
    {
      StringAssert.StartsWith(expectedDelimiter, actual);
      StringAssert.EndsWith(expectedDelimiter, actual);
    }

    public static void ShouldContainErrorMessage(this Exception exception, string expected)
    {
      StringAssert.Contains(expected, exception.Message);
    }

    public static void ShouldContainOnly<T>(this IEnumerable<T> actual, params T[] expected)
    {
      ShouldContainOnly(actual, new List<T>(expected));
    }

    public static void ShouldContainOnly<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
    {
      var actualList = new List<T>(actual);
      var remainingList = new List<T>(actualList);
      foreach (var item in expected)
      {
        Assert.Contains(item, actualList);
        remainingList.Remove(item);
      }
      Assert.IsEmpty(remainingList, "Actual collection has unexpected items.");
    }

    public static Exception ShouldBeThrownBy(this Type exceptionType, Action method)
    {
      Exception exception = method.GetException();

      Assert.IsNotNull(exception);
      Assert.AreEqual(exceptionType, exception.GetType());
      return exception;
    }

    static Exception GetException(this Action method)
    {
      Exception exception = null;

      try
      {
        method();
      }
      catch (Exception e)
      {
        exception = e;
      }

      return exception;
    }
  }
}