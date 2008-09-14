using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Machine.Specifications
{
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

    public static void ShouldNotContain(this IList collection, object expected)
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

    public static void ShouldBeEmpty(this ICollection collection)
    {
      Assert.IsEmpty(collection);
    }

    public static void ShouldBeEmpty(this string aString)
    {
      Assert.IsEmpty(aString);
    }

    public static void ShouldNotBeEmpty(this ICollection collection)
    {
      Assert.IsNotEmpty(collection);
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
      CollectionAssert.AreEqual(expected, actual);
    }

    public static void ShouldContainOnly<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
    {
      CollectionAssert.AreEqual(expected, actual);
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