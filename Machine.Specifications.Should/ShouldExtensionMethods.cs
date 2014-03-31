using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Machine.Specifications.Annotations;
using Machine.Specifications.Utility;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    public static class ShouldExtensionMethods
    {
        static bool SafeEquals<T>(this T left, T right)
        {
            var comparer = new AssertComparer<T>();

            return comparer.Compare(left, right) == 0;
        }

        [AssertionMethod]
        public static void ShouldBeFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] this bool condition)
        {
            if (condition)
                throw new SpecificationException("Should be [false] but is [true]");
        }

        [AssertionMethod]
        public static void ShouldBeTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool condition)
        {
            if (!condition)
                throw new SpecificationException("Should be [true] but is [false]");
        }

        public static T ShouldEqual<T>(this T actual, T expected)
        {
            if (!actual.SafeEquals(expected))
            {
                throw new SpecificationException(PrettyPrintingExtensions.FormatErrorMessage(actual, expected));
            }

            return actual;
        }

        public static object ShouldNotEqual<T>(this T actual, T expected)
        {
            if (actual.SafeEquals(expected))
            {
                throw new SpecificationException(string.Format("Should not equal {0} but does: {1}",
                    expected.ToUsefulString(),
                    actual.ToUsefulString()));
            }

            return actual;
        }

        [AssertionMethod]
        public static void ShouldBeNull([AssertionCondition(AssertionConditionType.IS_NULL)] this object anObject)
        {
            if (anObject != null)
            {
                throw new SpecificationException(string.Format((string)"Should be [null] but is {0}", (object)anObject.ToUsefulString()));
            }
        }

        [AssertionMethod]
        public static void ShouldNotBeNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this object anObject)
        {
            if (anObject == null)
            {
                throw new SpecificationException(string.Format("Should be [not null] but is [null]"));
            }
        }

        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            if (!ReferenceEquals(actual, expected))
            {
                throw new SpecificationException(string.Format("Should be the same as {0} but is {1}", expected, actual));
            }

            return expected;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            if (ReferenceEquals(actual, expected))
            {
                throw new SpecificationException(string.Format("Should not be the same as {0} but is {1}", expected, actual));
            }

            return expected;
        }

        [ObsoleteEx(Message = "This method behaved like `ShouldBeAssignableTo` which is misleading. For exact type comparison use `ShouldBeOfExactType`, for assignability verification use `ShouldBeAssignableTo`. Redesigned because of #171.", RemoveInVersion = "0.9", TreatAsErrorFromVersion = "0.8")]
        public static void ShouldBeOfType(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException(string.Format("Should be of type {0} but is [null]", expected));
            }

            if (!expected.IsAssignableFrom(actual.GetType()))
            {
                throw new SpecificationException(string.Format("Should be of type {0} but is of type {1}",
                    expected,
                    actual.GetType()));
            }
        }

        [ObsoleteEx(Message = "This method behaved like `ShouldBeAssignableTo` which is misleading. For exact type comparison use `ShouldBeOfExactType`, for assignability verification use `ShouldBeAssignableTo`. Redesigned because of #171.", RemoveInVersion = "0.9", TreatAsErrorFromVersion = "0.8")]
        public static void ShouldBeOfType<T>(this object actual)
        {
            actual.ShouldBeOfType(typeof(T));
        }

        [ObsoleteEx(Message = "This method behaved like `ShouldBeAssignableTo` which is misleading. For exact type comparison use `ShouldBeOfExactType`, for assignability verification use `ShouldBeAssignableTo`. Redesigned because of #171.", RemoveInVersion = "0.9", TreatAsErrorFromVersion = "0.8")]
        public static void ShouldBe(this object actual, Type expected)
        {
            actual.ShouldBeOfType(expected);
        }

        [ObsoleteEx(Message = "This method should no longer be used. Redesigned because of #171. See replacement.", RemoveInVersion = "0.9", TreatAsErrorFromVersion = "0.8", Replacement = "ShouldNotBeOfExactType")]
        public static void ShouldNotBeOfType(this object actual, Type expected)
        {
            if (actual.GetType() == expected)
            {
                throw new SpecificationException(string.Format("Should not be of type {0} but is of type {1}", expected,
                    actual.GetType()));
            }
        }

        public static void ShouldBeOfExactType(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException(string.Format("Should be of type {0} but is [null]", expected));
            }

            if (actual.GetType() != expected)
            {
                throw new SpecificationException(string.Format("Should be of type {0} but is of type {1}", expected,
                    actual.GetType()));
            }
        }

        public static void ShouldNotBeOfExactType(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException(string.Format("Should not be of type {0} but is [null]", expected));
            }

            if (actual.GetType() == expected)
            {
                throw new SpecificationException(string.Format("Should not be of type {0} but is of type {1}", expected,
                    actual.GetType()));
            }
        }

        public static void ShouldBeOfExactType<T>(this object actual)
        {
            actual.ShouldBeOfExactType(typeof(T));
        }

        public static void ShouldNotBeOfExactType<T>(this object actual)
        {
            actual.ShouldNotBeOfExactType(typeof(T));
        }

        public static void ShouldBeAssignableTo(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException(string.Format("Should be assignable to type {0} but is [null]", expected));
            }

            if (!expected.IsAssignableFrom(actual.GetType()))
            {
                throw new SpecificationException(string.Format("Should be assignable to type {0} but is not. Actual type is {1}",
                    expected,
                    actual.GetType()));
            }
        }

        public static void ShouldNotBeAssignableTo(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException(string.Format("Should not be assignable to type {0} but is [null]", expected));
            }

            if (expected.IsAssignableFrom(actual.GetType()))
            {
                throw new SpecificationException(string.Format("Should not be assignable to type {0} but is. Actual type is {1}",
                    expected,
                    actual.GetType()));
            }
        }

        public static void ShouldBeAssignableTo<T>(this object actual)
        {
            actual.ShouldBeAssignableTo(typeof(T));
        }

        public static void ShouldNotBeAssignableTo<T>(this object actual)
        {
            actual.ShouldNotBeAssignableTo(typeof(T));
        }

        public static void ShouldMatch<T>(this T actual, Expression<Func<T, bool>> condition)
        {
            var matches = condition.Compile().Invoke(actual);

            if (matches) return;
            throw new SpecificationException(string.Format("Should match expression [{0}], but does not.", condition));
        }

        public static void ShouldEachConformTo<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var source = new List<T>(list);
            var func = condition.Compile();

            var failingItems = source.Where(x => func(x) == false);

            if (failingItems.Any())
            {
                throw new SpecificationException(string.Format(
                                                               @"Should contain only elements conforming to: {0}
the following items did not meet the condition: {1}",
                    condition,
                    failingItems.EachToUsefulString()));
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
            list.ShouldContain((IEnumerable<T>)items);
        }

        public static void ShouldContain<T>(this IEnumerable<T> list, IEnumerable<T> items)
        {
            var noContain = new List<T>();
            var comparer = new AssertComparer<T>();

            foreach (var item in items)
            {
                if (!list.Contains<T>(item, comparer))
                {
                    noContain.Add(item);
                }
            }

            if (noContain.Any())
            {
                throw new SpecificationException(string.Format(
                                                               @"Should contain: {0} 
entire list: {1}
does not contain: {2}",
                    items.EachToUsefulString(),
                    list.EachToUsefulString(),
                    noContain.EachToUsefulString()));
            }
        }

        public static void ShouldContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();

            if (!list.Any(func))
            {
                throw new SpecificationException(string.Format(
                                                               @"Should contain elements conforming to: {0}
entire list: {1}",
                    condition,
                    list.EachToUsefulString()));
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
            list.ShouldNotContain((IEnumerable<T>)items);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, IEnumerable<T> items)
        {
            var contains = new List<T>();
            var comparer = new AssertComparer<T>();

            foreach (var item in items)
            {
                if (list.Contains<T>(item, comparer))
                {
                    contains.Add(item);
                }
            }

            if (contains.Any())
            {
                throw new SpecificationException(string.Format(
                                                               @"Should not contain: {0} 
entire list: {1}
does contain: {2}",
                    items.EachToUsefulString(),
                    list.EachToUsefulString(),
                    contains.EachToUsefulString()));
            }
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();

            var contains = list.Where(func);

            if (contains.Any())
            {
                throw new SpecificationException(string.Format(
                                                               @"No elements should conform to: {0}
entire list: {1}
does contain: {2}",
                    condition,
                    list.EachToUsefulString(),
                    contains.EachToUsefulString()));
            }
        }

        static SpecificationException NewException(string message, params object[] parameters)
        {
            if (parameters.Any())
            {
                return
                    new SpecificationException(string.Format(message.EnsureSafeFormat(),
                        parameters.Select<object, string>(x => x.ToUsefulString()).Cast<object>().ToArray()));
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

        public static IComparable ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null) throw new ArgumentNullException("arg2");
            if (arg1 == null) throw NewException("Should be greater than or equal to {0} but is [null]", arg2);

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) < 0)
                throw NewException("Should be greater than or equal to {0} but is {1}", arg2, arg1);

            return arg1;
        }

        static object TryToChangeType(this object original, Type type)
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
            if (arg1 == null) throw NewException("Should be less than {0} but is [null]", arg2);

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) >= 0)
                throw NewException("Should be less than {0} but is {1}", arg2, arg1);

            return arg1;
        }

        public static IComparable ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null) throw new ArgumentNullException("arg2");
            if (arg1 == null) throw NewException("Should be less than or equal to {0} but is [null]", arg2);

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) > 0)
                throw NewException("Should be less than or equal to {0} but is {1}", arg2, arg1);

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
                throw new SpecificationException(string.Format("Should be within {0} of {1} but is {2}",
                    tolerance.ToUsefulString(),
                    expected.ToUsefulString(),
                    actual.ToUsefulString()));
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
                throw new SpecificationException(string.Format("Should be within {0} of {1} but is {2}",
                    tolerance.ToUsefulString(),
                    expected.ToUsefulString(),
                    actual.ToUsefulString()));
            }
        }

        public static void ShouldBeCloseTo(this TimeSpan actual, TimeSpan expected, TimeSpan tolerance)
        {
            if (Math.Abs(actual.Ticks - expected.Ticks) > tolerance.Ticks)
            {
                throw new SpecificationException(string.Format("Should be within {0} of {1} but is {2}",
                    tolerance.ToUsefulString(),
                    expected.ToUsefulString(),
                    actual.ToUsefulString()));
            }
        }

        public static void ShouldBeCloseTo(this DateTime actual, DateTime expected, TimeSpan tolerance)
        {
            var difference = expected - actual;
            if (Math.Abs(difference.Ticks) > tolerance.Ticks)
            {
                throw new SpecificationException(string.Format("Should be within {0} of {1} but is {2}",
                    tolerance.ToUsefulString(),
                    expected.ToUsefulString(),
                    actual.ToUsefulString()));
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
            if (aString == null)
            {
                throw new SpecificationException("Should be empty but is [null]");
            }

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

            ShouldMatch(actual, new Regex(pattern));
        }

        public static void ShouldMatch(this string actual, Regex pattern)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            if (actual == null) throw NewException("Should match regex {0} but is [null]", pattern);

            if (!pattern.IsMatch(actual))
            {
                throw NewException("Should match {0} but is {1}", pattern, actual);
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
                if (!source.Contains<T>(item, comparer))
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
                var message = string.Format(@"Should contain only: {0} 
entire list: {1}",
                    items.EachToUsefulString(),
                    list.EachToUsefulString());
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
            var exception = Catch.Exception(method);

            ShouldNotBeNull(exception);
            ShouldBeOfType(exception, exceptionType);
            return exception;
        }

        public static void ShouldBeLike(this object obj, object expected)
        {
            var exceptions = ShouldBeLikeInternal(obj, expected, "").ToArray();

            if (exceptions.Any())
            {
                throw NewException(exceptions.Select(e => e.Message).Aggregate<string, string>("", (r, m) => r + m + Environment.NewLine + Environment.NewLine).TrimEnd());
            }
        }

        static IEnumerable<SpecificationException> ShouldBeLikeInternal(object obj, object expected, string nodeName)
        {
            ObjectGraphHelper.INode expectedNode = null;
            var nodeType = typeof(ObjectGraphHelper.LiteralNode);

            if (expected != null)
            {
                expectedNode = ObjectGraphHelper.GetGraph(expected);
                nodeType = expectedNode.GetType();
            }

            if (nodeType == typeof(ObjectGraphHelper.LiteralNode))
            {
                try
                {
                    obj.ShouldEqual(expected);
                }
                catch (SpecificationException ex)
                {
                    return new[] { NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, ex.Message), nodeName) };
                }

                return Enumerable.Empty<SpecificationException>();
            }
            else if (nodeType == typeof(ObjectGraphHelper.SequenceNode))
            {
                if (obj == null)
                {
                    var errorMessage = PrettyPrintingExtensions.FormatErrorMessage(null, expected);
                    return new[] { NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName) };
                }
                var actualNode = ObjectGraphHelper.GetGraph(obj);
                if (actualNode.GetType() != typeof(ObjectGraphHelper.SequenceNode))
                {
                    var errorMessage = string.Format("  Expected: Array or Sequence{0}  But was:  {1}", Environment.NewLine, obj.GetType());
                    return new[] { NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName) };
                }

                var expectedValues = ((ObjectGraphHelper.SequenceNode)expectedNode).ValueGetters;
                var actualValues = ((ObjectGraphHelper.SequenceNode)actualNode).ValueGetters;

                var expectedCount = Enumerable.Count<Func<object>>(expectedValues);
                var actualCount = Enumerable.Count<Func<object>>(actualValues);
                if (expectedCount != actualCount)
                {
                    var errorMessage = string.Format("  Expected: Sequence length of {1}{0}  But was:  {2}", Environment.NewLine, expectedCount, actualCount);
                    return new[] { NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName) };
                }

                return Enumerable.Range(0, expectedCount)
                    .SelectMany(i => ShouldBeLikeInternal(Enumerable.ElementAt<Func<object>>(actualValues, i)(),
                        Enumerable.ElementAt<Func<object>>(expectedValues, i)(),
                        string.Format("{0}[{1}]", nodeName, i)));
            }
            else if (nodeType == typeof(ObjectGraphHelper.KeyValueNode))
            {
                var actualNode = ObjectGraphHelper.GetGraph(obj);
                if (actualNode.GetType() != typeof(ObjectGraphHelper.KeyValueNode))
                {
                    var errorMessage = string.Format("  Expected: Class{0}  But was:  {1}", Environment.NewLine, obj.GetType());
                    return new[] { NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), nodeName) };
                }

                var expectedKeyValues = ((ObjectGraphHelper.KeyValueNode)expectedNode).KeyValues;
                var actualKeyValues = ((ObjectGraphHelper.KeyValueNode)actualNode).KeyValues;

                return expectedKeyValues
                    .SelectMany(kv =>
                    {
                        var fullNodeName = string.IsNullOrEmpty(nodeName) ? kv.Name : string.Format("{0}.{1}", nodeName, kv.Name);
                        var actualKeyValue = actualKeyValues.SingleOrDefault(k => k.Name == kv.Name);
                        if (actualKeyValue == null)
                        {
                            var errorMessage = string.Format("  Expected: {1}{0}  But was:  Not Defined", Environment.NewLine, kv.ValueGetter().ToUsefulString());
                            return new[] { NewException(string.Format("{{0}}:{0}{1}", Environment.NewLine, errorMessage), fullNodeName) };
                        }

                        return ShouldBeLikeInternal(actualKeyValue.ValueGetter(), kv.ValueGetter(), fullNodeName);
                    });
            }
            else
            {
                throw new InvalidOperationException("Unknown node type");
            }
        }
    }
}
