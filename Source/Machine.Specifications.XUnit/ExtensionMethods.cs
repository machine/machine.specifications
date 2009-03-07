using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Xunit;

namespace Machine.Specifications
{
  public static class XUnitCollectionExtensionMethods
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
  public static class XUnitShouldExtensionMethods
  {
    public static void ShouldBeFalse(this bool condition)
    {
      Assert.False(condition);
    }

    public static void ShouldBeTrue(this bool condition)
    {
      Assert.True(condition);
    }

    public static object ShouldEqual<T>(this T actual, T expected)
    {
      Assert.Equal(expected, actual);
      return expected;
    }

    public static object ShouldNotEqual<T>(this T actual, T expected)
    {
      Assert.NotEqual(expected, actual);
      return expected;
    }

    public static void ShouldBeNull(this object anObject)
    {
      Assert.Null(anObject);
    }

    public static void ShouldNotBeNull(this object anObject)
    {
      Assert.NotNull(anObject);
    }

    public static object ShouldBeTheSameAs(this object actual, object expected)
    {
      Assert.Same(expected, actual);
      return expected;
    }

    public static object ShouldNotBeTheSameAs(this object actual, object expected)
    {
      Assert.NotSame(expected, actual);
      return expected;
    }

    public static void ShouldBeOfType(this object actual, Type expected)
    {
      Assert.IsType(expected, actual);
    }

    public static void ShouldBeOfType<T>(this object actual)
    {
      Assert.IsType<T>(actual);
    }

    public static void ShouldBe(this object actual, Type expected)
    {
      Assert.IsType(expected, actual);
    }

    public static void ShouldNotBeOfType(this object actual, Type expected)
    {
      Assert.IsNotType(expected, actual);
    }

    public static void ShouldContain(this IList actual, params object[] expected)
    {
      foreach (var item in expected)
      {
        Assert.Contains(item, actual.Cast<object>());
      }
    }

    public static void ShouldContain<T>(this IEnumerable<T> actual, params T[] expected)
    {
      foreach (var item in expected)
      {
        Assert.Contains(item, actual);
      }
    }

    public static void ShouldNotContain(this IEnumerable collection, object expected)
    {
      Assert.DoesNotContain(expected, collection.Cast<object>());
    }

    public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
    {
      Assert.Equal(1, arg1.CompareTo(arg2));
      return arg2;
    }

    public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
    {
      Assert.Equal(-1, arg1.CompareTo(arg2));
      return arg2;
    }

    public static void ShouldBeEmpty(this IEnumerable collection)
    {
      Assert.Empty(collection.ToArrayList());
    }

    public static void ShouldBeEmpty(this string aString)
    {
      Assert.Empty(aString);
    }

    public static void ShouldNotBeEmpty(this IEnumerable collection)
    {
      Assert.NotEmpty(collection.ToArrayList());
    }

    public static void ShouldNotBeEmpty(this string aString)
    {
      Assert.NotEmpty(aString);
    }

    public static void ShouldContain(this string actual, string expected)
    {
      Assert.Contains(expected, actual);
    }

    public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
    {
      Assert.Equal(expected.ToLowerInvariant(), actual.ToLowerInvariant());
      return expected;
    }

    public static void ShouldStartWith(this string actual, string expected)
    {
      Assert.True(actual.StartsWith(expected));
    }

    public static void ShouldEndWith(this string actual, string expected)
    {
      Assert.True(actual.EndsWith(expected));
    }

    public static void ShouldBeSurroundedWith(this string actual, string expectedStartDelimiter,
      string expectedEndDelimiter)
    {
      Assert.True(actual.StartsWith(expectedStartDelimiter));
	  Assert.True(actual.EndsWith(expectedEndDelimiter));
    }

    public static void ShouldBeSurroundedWith(this string actual, string expectedDelimiter)
    {
      Assert.True(actual.StartsWith(expectedDelimiter));
	  Assert.True(actual.EndsWith(expectedDelimiter));
    }

    public static void ShouldContainErrorMessage(this Exception exception, string expected)
    {
      Assert.Contains(expected, exception.Message);
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
      Assert.Empty(remainingList);
    }

    public static Exception ShouldBeThrownBy(this Type exceptionType, Action method)
    {
      Exception exception = method.GetException();

      Assert.NotNull(exception);
      Assert.Equal(exceptionType, exception.GetType());
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