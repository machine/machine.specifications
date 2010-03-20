using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace Machine.Specifications
{
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

  public static class ShouldExtensionMethods
  {
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
      string str;
      if (obj == null) return "[null]";
      if (obj.GetType() == typeof(string))
      {
        str = (string) obj;

        return "\"" + str.Replace("\n", "\\n") + "\"";
      }
      if (obj.GetType().IsValueType) return "[" + obj + "]";

      if (obj is IEnumerable)
      {
        var enumerable = ((IEnumerable)obj).Cast<object>();

        return obj.GetType() + ":\n" + enumerable.EachToUsefulString();
      }

      str = obj.ToString();

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

      if (obj.GetType().ToString() == str)
        return obj.GetType().ToString();

      return string.Format("{0}:[{1}]", obj.GetType(), str);
    }

    private static bool SafeEquals<T>(this T left, T right)
    {
      var comparer = new AssertComparer<T>();

      return comparer.Compare(left, right) == 0;
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

    public static T ShouldEqual<T>(this T actual, T expected)
    {
      if (!actual.SafeEquals(expected))
      {
        throw new SpecificationException(string.Format("Should equal {0} but is {1}", expected.ToUsefulString(), actual.ToUsefulString()));
      }

      return actual;
    }

    public static object ShouldNotEqual<T>(this T actual, T expected)
    {
      if (actual.SafeEquals(expected))
      {
        throw new SpecificationException(string.Format("Should not equal {0} but does: {1}", expected.ToUsefulString(), actual.ToUsefulString()));
      }

      return actual;
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
      if (actual == null) throw new SpecificationException(string.Format("Should be of type {0} but is [null]", expected.GetType()));
      if (!expected.IsAssignableFrom(actual.GetType()))
      {
        throw new SpecificationException(string.Format("Should be of type {0} but is of type {1}", expected, actual.GetType()));
      }
    }

    public static void ShouldBeOfType<T>(this object actual)
    {
      actual.ShouldBeOfType(typeof(T));
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
    
    public static void ShouldEachConformTo<T>(this IEnumerable<T> list, Func<T,bool> condition)
    {
      var source = new List<T>(list);

      var failingItems = source.Where(x => condition(x) == false);

      if(failingItems.Any())
      {
        var message = string.Format(@"The following elements did not conform to the specified condition: {0}", 
          failingItems.EachToUsefulString());

        throw new SpecificationException(message);
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
      var comparer = new AssertComparer<T>();

      foreach (var item in items)
      {
        if (!list.Contains(item, comparer))
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
      var comparer = new AssertComparer<T>();

      foreach (var item in items)
      {
        if (list.Contains(item, comparer))
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

      if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) <= 0)
        throw NewException("Should be greater than {0} but is {1}", arg2, arg1);
        
      return arg1;
    }

    private static object TryToChangeType(this object original, Type type)
    {
      try
      {
        return Convert.ChangeType(original, type);
      }
      catch 
      {
        return original;
      }
    }

    public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
    {
      if (arg2 == null) throw new ArgumentNullException("arg2");
      if (arg1 == null) throw NewException("Should be greater than {0} but is [null]", arg2);

      if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) >= 0)
        throw NewException("Should be less than {0} but is {1}", arg2, arg1);
        
      return arg1;
    }

    public static void ShouldBeCloseTo(this float actual, float expected)
    {
      ShouldBeCloseTo(actual, expected, 0.0000001f);
    }

    public static void ShouldBeCloseTo(this float actual, float expected, float tolerance)
    {
      if (Math.Abs(actual - expected) > tolerance)
      {
        throw new SpecificationException(string.Format("Should be within {0} of {1} but is {2}", tolerance.ToUsefulString(), expected.ToUsefulString(), actual.ToUsefulString()));
      }
    }

    public static void ShouldBeCloseTo(this double actual, double expected)
    {
      ShouldBeCloseTo(actual, expected, 0.0000001f);
    }

    public static void ShouldBeCloseTo(this double actual, double expected, double tolerance)
    {
      if (Math.Abs(actual - expected) > tolerance)
      {
        throw new SpecificationException(string.Format("Should be within {0} of {1} but is {2}", tolerance.ToUsefulString(), expected.ToUsefulString(), actual.ToUsefulString()));
      }
    }

    public static void ShouldBeCloseTo(this TimeSpan actual, TimeSpan expected, TimeSpan tolerance)
    {
      if (Math.Abs(actual.Ticks - expected.Ticks) > tolerance.Ticks)
      {
        throw new SpecificationException(string.Format("Should be within {0} of {1} but is {2}", tolerance.ToUsefulString(), expected.ToUsefulString(), actual.ToUsefulString()));
      }
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

    public static void ShouldMatch(this string actual, string pattern)
    {
      if (pattern == null) throw new ArgumentNullException("pattern");
      if (actual == null) throw NewException("Should match regex {0} but is [null]", pattern);

      ShouldMatch(actual, new System.Text.RegularExpressions.Regex(pattern));
    }

    public static void ShouldMatch(this string actual, System.Text.RegularExpressions.Regex pattern)
    {
      if (pattern == null) throw new ArgumentNullException("pattern");
      if (actual == null) throw NewException("Should match regex {0} but is [null]", pattern);

      if (!pattern.IsMatch(actual))
        throw NewException("Should match {0} but is {1}", pattern, actual);        
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

    public static void ShouldNotContain(this string actual, string notExpected)
    {
      if (notExpected == null) throw new ArgumentNullException("notExpected");
      if (actual == null) return;

      if (actual.Contains(notExpected))
      {
        throw NewException("Should not contain {0} but is {1}", notExpected, actual);
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
      var comparer = new AssertComparer<T>();

      foreach (var item in items)
      {
        if (!source.Contains(item, comparer))
        {
          noContain.Add(item);
        }
        else
        {
          source.Remove(item);
        }
      }

      if (noContain.Any() || source.Any())
      {
        string message = string.Format(@"Should contain only: {0} 
entire list: {1}", items.EachToUsefulString(), list.EachToUsefulString());
        if (noContain.Any())
        {
          message += "\ndoes not contain: " + noContain.EachToUsefulString();
        }
        if (source.Any())
        {
          message += "\ndoes contain but shouldn't: " + source.EachToUsefulString();
        }

        throw new SpecificationException(message);
      }
    }

    public static Exception ShouldBeThrownBy(this Type exceptionType, Action method)
    {
      Exception exception = Catch.Exception(method);

      exception.ShouldNotBeNull();
      exception.ShouldBeOfType(exceptionType);
      return exception;
    }
  }
}