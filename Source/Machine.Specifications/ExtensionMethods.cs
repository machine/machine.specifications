using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace Machine.Specifications
{
  public static class NUnitCollectionExtensionMethods
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
  }

  [Serializable]
  public class SpecificationException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public SpecificationException()
    {
    }

    public SpecificationException(string message)
      : base(message)
    {
    }

    public SpecificationException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected SpecificationException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }

  public class TestFoo
  {
    public override string ToString()
    {
      return @"Hello
there 
how are you 
today?";
    }
  }

  public static class NUnitShouldExtensionMethods
  {
    public static void Temp2()
    {
      1.ShouldBeGreaterThan(2);
    }
    public static void Temp()
    {
      new object[] {new TestFoo(), 
        null, 
        null, 
        null, 
        null, 
        null, 
        null, 
        null, 
        null, 
        null, 
        null, 
        new TestFoo()}.ShouldBeEmpty();
    }
    private static string Tab(this string str)
    {
      if (string.IsNullOrEmpty(str)) return "";

      var split = str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
      var sb = new StringBuilder();

      sb.Append("  " + split[0]);
      foreach (var part in split.Skip(1))
      {
        sb.AppendLine();
        sb.Append("  " + part);
      }

      return sb.ToString();
    }

    private static string EachToUsefulString<T>(this IEnumerable<T> enumerable)
    {
      var sb = new StringBuilder();
      sb.AppendLine("{");
      sb.Append(String.Join(",\n", enumerable.Select(x => x.ToUsefulString().Tab()).Take(10).ToArray()));
      if (enumerable.Count() > 10)
      {
        if (enumerable.Count() > 11)
        {
          sb.AppendLine(String.Format(",\n  ...({0} more elements)", enumerable.Count() - 10));
        }
        else
        {
          sb.AppendLine(",\n" + enumerable.Last().ToUsefulString().Tab());
        }
      }
      else sb.AppendLine();
      sb.AppendLine("}");

      return sb.ToString();
    }

    private static string ToUsefulString(this object obj)
    {
      if (obj == null) return "[null]";
      if (obj.GetType() == typeof(string)) return "\"" + obj + "\"";
      if (obj.GetType().IsValueType) return "[" + obj + "]";

      if (obj is IEnumerable)
      {
        var enumerable = ((IEnumerable)obj).Cast<object>();

        return obj.GetType() + ":\n" + enumerable.EachToUsefulString();
      }

      var str = obj.ToString();

      if (str == null || str.Trim() == "")
      {
        return String.Format("{0}:[]", obj.GetType());
      }

      str = str.Trim();

      if (str.Contains("\n"))
      {
        return string.Format(@"{1}:
[
{0}
]", str.Tab(), obj.GetType());
      }

      return string.Format("{0}:[{1}]", obj.GetType(), str);
    }

    private static bool SafeEquals(this object left, object right)
    {
      if (left == null && right != null) return false;
      if (right == null && left != null) return false;
      if (left == null) return true;

      return left.Equals(right);
    }

    public static void ShouldBeFalse(this bool condition)
    {
      if (condition)
        throw new SpecificationException("Should be [false] but is [true]");
    }

    public static void ShouldBeTrue(this bool condition)
    {
      if (!condition)
        throw new SpecificationException("Should be [true] but is [false]");
    }

    public static object ShouldEqual(this object actual, object expected)
    {
      if (!actual.SafeEquals(expected))
      {
        throw new SpecificationException(string.Format("Should equal {0} but is {1}", expected.ToUsefulString(), actual.ToUsefulString()));
      }

      return expected;
    }

    public static object ShouldNotEqual(this object actual, object expected)
    {
      if (actual.SafeEquals(expected))
      {
        throw new SpecificationException(string.Format("Should not equal {0} but does: {1}", expected.ToUsefulString(), actual.ToUsefulString()));
      }

      return expected;
    }

    public static void ShouldBeNull(this object anObject)
    {
      if (anObject != null)
      {
        throw new SpecificationException(string.Format("Should be [null] but is {0}", anObject.ToUsefulString()));
      }
    }

    public static void ShouldNotBeNull(this object anObject)
    {
      if (anObject == null)
      {
        throw new SpecificationException(string.Format("Should be [not null] but is [null]"));
      }
    }

    public static object ShouldBeTheSameAs(this object actual, object expected)
    {
      if (!Object.ReferenceEquals(actual, expected))
      {
        throw new SpecificationException(string.Format("Should be the same as {0} but is {1}", expected, actual));
      }

      return expected;
    }

    public static object ShouldNotBeTheSameAs(this object actual, object expected)
    {
      if (Object.ReferenceEquals(actual, expected))
      {
        throw new SpecificationException(string.Format("Should not be the same as {0} but is {1}", expected, actual));
      }

      return expected;

    }

    public static void ShouldBeOfType(this object actual, Type expected)
    {
      if (actual.GetType() != expected)
      {
        throw new SpecificationException(string.Format("Should be of type {0} but is of type {1}", expected, actual.GetType()));
      }
    }

    public static void ShouldBeOfType<T>(this object actual)
    {
      if (actual.GetType() != typeof(T))
      {
        throw new SpecificationException(string.Format("Should be of type {0} but is of type {1}", typeof(T), actual.GetType()));
      }
    }

    public static void ShouldBe(this object actual, Type expected)
    {
      actual.ShouldBeOfType(expected);
    }

    public static void ShouldNotBeOfType(this object actual, Type expected)
    {
      if (actual.GetType() == expected)
      {
        throw new SpecificationException(string.Format("Should not be of type {0} but is of type {1}", expected, actual.GetType()));
      }
    }

    public static void ShouldContain(this IEnumerable list, params object[] items)
    {
      var actualList = list.Cast<object>();
      var expectedList = items.Cast<object>();

      actualList.ShouldContain(expectedList);
    }

    public static void ShouldContain<T>(this IEnumerable<T> list, params T[] items)
    {
      var noContain = new List<T>();

      foreach (var item in items)
      {
        if (!list.Contains(item))
        {
          noContain.Add(item);
        }
      }

      if (noContain.Any())
      {
        throw new SpecificationException(string.Format(@"Should contain: {0} 
entire list: {1}
does not contain: {2}", items.EachToUsefulString(), list.EachToUsefulString(), noContain.EachToUsefulString()));
      }
    }

    public static void ShouldNotContain(this IEnumerable list, params object[] items)
    {
      var actualList = list.Cast<object>();
      var expectedList = items.Cast<object>();

      actualList.ShouldNotContain(expectedList);
    }

    public static void ShouldNotContain<T>(this IEnumerable<T> list, params T[] items)
    {
      var contains = new List<T>();

      foreach (var item in items)
      {
        if (list.Contains(item))
        {
          contains.Add(item);
        }
      }

      if (contains.Any())
      {
        throw new SpecificationException(string.Format(@"Should not contain: {0} 
entire list: {1}
does contain: {2}", items.EachToUsefulString(), list.EachToUsefulString(), contains.EachToUsefulString()));
      }
    }

    private static SpecificationException NewException(string message, params object[] parameters)
    {
      if (parameters.Any())
      {
        return new SpecificationException(string.Format(message, parameters.Select(x => x.ToUsefulString()).Cast<object>().ToArray() ));
      }
      return new SpecificationException(message);
    }

    public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
    {
      if (arg2 == null) throw new ArgumentNullException("arg2");
      if (arg1 == null) throw NewException("Should be greater than {0} but is [null]", arg2);

      if (arg1.CompareTo(arg2) <= 0)
        throw NewException("Should be greater than {0} but is {1}", arg2, arg1);
        
      return arg1;
    }

    public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
    {
      if (arg2 == null) throw new ArgumentNullException("arg2");
      if (arg1 == null) throw NewException("Should be greater than {0} but is [null]", arg2);

      if (arg1.CompareTo(arg2) >= 0)
        throw NewException("Should be less than {0} but is {1}", arg2, arg1);
        
      return arg1;
    }

    public static void ShouldBeEmpty(this IEnumerable collection)
    {
      if (collection.Cast<object>().Any())
      {
        throw NewException("Should be empty but contains:\n" + collection.Cast<object>().EachToUsefulString());
      }
    }

    public static void ShouldBeEmpty(this string aString)
    {
      if (!string.IsNullOrEmpty(aString))
      {
        throw NewException("Should be empty but is {0}", aString);
      }
    }

    public static void ShouldNotBeEmpty(this IEnumerable collection)
    {
      if (!collection.Cast<object>().Any())
      {
        throw NewException("Should not be empty but is");
      }
    }

    public static void ShouldNotBeEmpty(this string aString)
    {
      if (string.IsNullOrEmpty(aString))
      {
        throw NewException("Should not be empty but is");
      }
    }

    public static void ShouldContain(this string actual, string expected)
    {
      if (expected == null) throw new ArgumentNullException("expected");
      if (actual == null) throw NewException("Should contain {0} but is [null]", expected);

      if (!actual.Contains(expected))
      {
        throw NewException("Should contain {0} but is {1}", expected, actual);
      }
    }

    public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
    {
      if (expected == null) throw new ArgumentNullException("expected");
      if (actual == null) throw NewException("Should be equal ignoring case to {0} but is [null]", expected);

      if (!actual.Equals(expected, StringComparison.InvariantCultureIgnoreCase))
      {
        throw NewException("Should be equal ignoring case to {0} but is {1}", expected, actual);
      }

      return actual;
    }

    public static void ShouldStartWith(this string actual, string expected)
    {
      if (expected == null) throw new ArgumentNullException("expected");
      if (actual == null) throw NewException("Should start with {0} but is [null]", expected);

      if (!actual.StartsWith(expected))
      {
        throw NewException("Should start with {0} but is {1}", expected, actual);
      }
    }

    public static void ShouldEndWith(this string actual, string expected)
    {
      if (expected == null) throw new ArgumentNullException("expected");
      if (actual == null) throw NewException("Should end with {0} but is [null]", expected);

      if (!actual.EndsWith(expected))
      {
        throw NewException("Should end with {0} but is {1}", expected, actual);
      }
    }

    public static void ShouldBeSurroundedWith(this string actual, string expectedStartDelimiter,
      string expectedEndDelimiter)
    {
      actual.ShouldStartWith(expectedStartDelimiter);
      actual.ShouldEndWith(expectedEndDelimiter);
    }

    public static void ShouldBeSurroundedWith(this string actual, string expectedDelimiter)
    {
      actual.ShouldStartWith(expectedDelimiter);
      actual.ShouldEndWith(expectedDelimiter);
    }

    public static void ShouldContainErrorMessage(this Exception exception, string expected)
    {
      exception.Message.ShouldContain(expected);
    }

    public static void ShouldContainOnly<T>(this IEnumerable<T> list, params T[] items)
    {
      list.ShouldContainOnly((IEnumerable<T>)items);
    }

    public static void ShouldContainOnly<T>(this IEnumerable<T> list, IEnumerable<T> items)
    {
      var source = new List<T>(list);
      var noContain = new List<T>();

      foreach (var item in items)
      {
        if (!source.Contains(item))
        {
          noContain.Add(item);
        }
        else
        {
          source.Remove(item);
        }
      }

      if (noContain.Any())
      {
        throw new SpecificationException(string.Format(@"Should contain only: {0} 
entire list: {1}
does not contain: {2}
does contain but shouldn't: {3}", items.EachToUsefulString(), list.EachToUsefulString(), noContain.EachToUsefulString(), source.EachToUsefulString()));
      }
    }

    [Obsolete]
    public static Exception ShouldBeThrownBy(this Type exceptionType, Action method)
    {
      Exception exception = Catch.Exception(method);

      exception.ShouldNotBeNull();
      exception.ShouldBeOfType(exceptionType);
      return exception;
    }
  }
}