using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Machine.Specifications.Annotations;
using Machine.Specifications.Utility;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    public static class ShouldExtensionMethods
    {
        private static bool SafeEquals<T>(this T left, T right)
        {
            var comparer = new AssertComparer<T>();

            return comparer.Compare(left, right) == 0;
        }

        [AssertionMethod]
        public static void ShouldBeFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] this bool condition)
        {
            if (condition)
            {
                throw new SpecificationException("Should be [false] but is [true]");
            }
        }

        [AssertionMethod]
        public static void ShouldBeTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool condition)
        {
            if (!condition)
            {
                throw new SpecificationException("Should be [true] but is [false]");
            }
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
                throw new SpecificationException($"Should not equal {expected.ToUsefulString()} but does: {actual.ToUsefulString()}");
            }

            return actual;
        }

        [AssertionMethod]
        public static void ShouldBeNull([AssertionCondition(AssertionConditionType.IS_NULL)] this object anObject)
        {
            if (anObject != null)
            {
                throw new SpecificationException($"Should be [null] but is {anObject.ToUsefulString()}");
            }
        }

        [AssertionMethod]
        public static void ShouldNotBeNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this object anObject)
        {
            if (anObject == null)
            {
                throw new SpecificationException("Should be [not null] but is [null]");
            }
        }

        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            if (!ReferenceEquals(actual, expected))
            {
                throw new SpecificationException($"Should be the same as {expected} but is {actual}");
            }

            return expected;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            if (ReferenceEquals(actual, expected))
            {
                throw new SpecificationException($"Should not be the same as {expected} but is {actual}");
            }

            return expected;
        }

        public static void ShouldBeOfExactType(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException($"Should be of type {expected} but is [null]");
            }

            if (actual.GetType() != expected)
            {
                throw new SpecificationException($"Should be of type {expected} but is of type {actual.GetType()}");
            }
        }

        public static void ShouldNotBeOfExactType(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException($"Should not be of type {expected} but is [null]");
            }

            if (actual.GetType() == expected)
            {
                throw new SpecificationException($"Should not be of type {expected} but is of type {actual.GetType()}");
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
                throw new SpecificationException($"Should be assignable to type {expected} but is [null]");
            }

            if (!expected.IsInstanceOfType(actual))
            {
                throw new SpecificationException($"Should be assignable to type {expected} but is not. Actual type is {actual.GetType()}");
            }
        }

        public static void ShouldNotBeAssignableTo(this object actual, Type expected)
        {
            if (actual == null)
            {
                throw new SpecificationException($"Should not be assignable to type {expected} but is [null]");
            }

            if (expected.IsInstanceOfType(actual))
            {
                throw new SpecificationException($"Should not be assignable to type {expected} but is. Actual type is {actual.GetType()}");
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

            if (matches)
            {
                return;
            }

            throw new SpecificationException($"Should match expression [{condition}], but does not.");
        }

        public static void ShouldEachConformTo<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var source = new List<T>(list);
            var func = condition.Compile();

            var failingItems = source
                .Where(x => func(x) == false)
                .ToArray();

            if (failingItems.Any())
            {
                throw new SpecificationException(string.Format(@"Should contain only elements conforming to: {0}" + Environment.NewLine + "the following items did not meet the condition: {1}",
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
            var comparer = new AssertComparer<T>();

            var listArray = list.ToArray();
            var itemsArray = items.ToArray();

            var noContain = itemsArray
                .Where(x => !listArray.Contains(x, comparer))
                .ToList();

            if (noContain.Any())
            {
                throw new SpecificationException(string.Format(
                    @"Should contain: {0}" + Environment.NewLine +
                    "entire list: {1}" + Environment.NewLine +
                    "does not contain: {2}",
                    itemsArray.EachToUsefulString(),
                    listArray.EachToUsefulString(),
                    noContain.EachToUsefulString()));
            }
        }

        public static void ShouldContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();
            var listArray = list.ToArray();

            if (!listArray.Any(func))
            {
                throw new SpecificationException(string.Format(
                    @"Should contain elements conforming to: {0}" + Environment.NewLine +
                    "entire list: {1}",
                    condition,
                    listArray.EachToUsefulString()));
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
            var comparer = new AssertComparer<T>();

            var listArray = list.ToArray();
            var itemsArray = items.ToArray();

            var contains = itemsArray
                .Where(x => listArray.Contains(x, comparer))
                .ToList();

            if (contains.Any())
            {
                throw new SpecificationException(string.Format(
                    @"Should not contain: {0}" + Environment.NewLine +
                    "entire list: {1}" + Environment.NewLine +
                    "does contain: {2}",
                    itemsArray.EachToUsefulString(),
                    listArray.EachToUsefulString(),
                    contains.EachToUsefulString()));
            }
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();

            var listArray = list.ToArray();
            var contains = listArray.Where(func).ToArray();

            if (contains.Any())
            {
                throw new SpecificationException(string.Format(
                    @"No elements should conform to: {0}" + Environment.NewLine +
                    "entire list: {1}" + Environment.NewLine +
                    "does contain: {2}",
                    condition,
                    listArray.EachToUsefulString(),
                    contains.EachToUsefulString()));
            }
        }

        private static SpecificationException NewException(string message, params object[] parameters)
        {
            if (parameters.Any())
            {
                return new SpecificationException(string.Format(message.EnsureSafeFormat(), parameters.Select(x => x.ToUsefulString()).Cast<object>().ToArray()));
            }

            return new SpecificationException(message);
        }

        public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be greater than {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) <= 0)
            {
                throw NewException("Should be greater than {0} but is {1}", arg2, arg1);
            }

            return arg1;
        }

        public static IComparable ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be greater than or equal to {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) < 0)
            {
                throw NewException("Should be greater than or equal to {0} but is {1}", arg2, arg1);
            }

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
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be less than {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) >= 0)
            {
                throw NewException("Should be less than {0} but is {1}", arg2, arg1);
            }

            return arg1;
        }

        public static IComparable ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            if (arg2 == null)
            {
                throw new ArgumentNullException(nameof(arg2));
            }

            if (arg1 == null)
            {
                throw NewException("Should be less than or equal to {0} but is [null]", arg2);
            }

            if (arg1.CompareTo(arg2.TryToChangeType(arg1.GetType())) > 0)
            {
                throw NewException("Should be less than or equal to {0} but is {1}", arg2, arg1);
            }

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
                throw new SpecificationException($"Should be within {tolerance.ToUsefulString()} of {expected.ToUsefulString()} but is {actual.ToUsefulString()}");
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
                throw new SpecificationException($"Should be within {tolerance.ToUsefulString()} of {expected.ToUsefulString()} but is {actual.ToUsefulString()}");
            }
        }

        public static void ShouldBeCloseTo(this decimal actual, decimal expected)
        {
            ShouldBeCloseTo(actual, expected, 0.0000001m);
        }

        public static void ShouldBeCloseTo(this decimal actual, decimal expected, decimal tolerance)
        {
            if (Math.Abs(actual - expected) > tolerance)
            {
                throw new SpecificationException($"Should be within {tolerance.ToUsefulString()} of {expected.ToUsefulString()} but is {actual.ToUsefulString()}");
            }
        }

        public static void ShouldBeCloseTo(this TimeSpan actual, TimeSpan expected, TimeSpan tolerance)
        {
            if (Math.Abs(actual.Ticks - expected.Ticks) > tolerance.Ticks)
            {
                throw new SpecificationException($"Should be within {tolerance.ToUsefulString()} of {expected.ToUsefulString()} but is {actual.ToUsefulString()}");
            }
        }

        public static void ShouldBeCloseTo(this DateTime actual, DateTime expected, TimeSpan tolerance)
        {
            var difference = expected - actual;

            if (Math.Abs(difference.Ticks) > tolerance.Ticks)
            {
                throw new SpecificationException($"Should be within {tolerance.ToUsefulString()} of {expected.ToUsefulString()} but is {actual.ToUsefulString()}");
            }
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            var items = collection.Cast<object>().ToArray();

            if (items.Any())
            {
                throw NewException("Should be empty but contains:\n" + items.EachToUsefulString());
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
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (actual == null)
            {
                throw NewException("Should match regex {0} but is [null]", pattern);
            }

            ShouldMatch(actual, new Regex(pattern));
        }

        public static void ShouldMatch(this string actual, Regex pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (actual == null)
            {
                throw NewException("Should match regex {0} but is [null]", pattern);
            }

            if (!pattern.IsMatch(actual))
            {
                throw NewException("Should match {0} but is {1}", pattern, actual);
            }
        }

        public static void ShouldContain(this string actual, string expected)
        {
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual == null)
            {
                throw NewException("Should contain {0} but is [null]", expected);
            }

            if (!actual.Contains(expected))
            {
                throw NewException("Should contain {0} but is {1}", expected, actual);
            }
        }

        public static void ShouldNotContain(this string actual, string notExpected)
        {
            if (notExpected == null)
            {
                throw new ArgumentNullException(nameof(notExpected));
            }

            if (actual == null)
            {
                return;
            }

            if (actual.Contains(notExpected))
            {
                throw NewException("Should not contain {0} but is {1}", notExpected, actual);
            }
        }

        public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
        {
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual == null)
            {
                throw NewException("Should be equal ignoring case to {0} but is [null]", expected);
            }

            if (CultureInfo.InvariantCulture.CompareInfo.Compare(actual, expected, CompareOptions.IgnoreCase) != 0)
            {
                throw NewException("Should be equal ignoring case to {0} but is {1}", expected, actual);
            }

            return actual;
        }

        public static void ShouldStartWith(this string actual, string expected)
        {
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual == null)
            {
                throw NewException("Should start with {0} but is [null]", expected);
            }

            if (!actual.StartsWith(expected))
            {
                throw NewException("Should start with {0} but is {1}", expected, actual);
            }
        }

        public static void ShouldEndWith(this string actual, string expected)
        {
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual == null)
            {
                throw NewException("Should end with {0} but is [null]", expected);
            }

            if (!actual.EndsWith(expected))
            {
                throw NewException("Should end with {0} but is {1}", expected, actual);
            }
        }

        public static void ShouldBeSurroundedWith(this string actual, string expectedStartDelimiter, string expectedEndDelimiter)
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
            var listArray = list.ToArray();
            var itemsArray = items.ToArray();

            var source = new List<T>(listArray);
            var noContain = new List<T>();
            var comparer = new AssertComparer<T>();

            foreach (var item in itemsArray)
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
                var message = string.Format(@"Should contain only: {0}" + Environment.NewLine + "entire list: {1}",
                    itemsArray.EachToUsefulString(),
                    listArray.EachToUsefulString());

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
            var exception = CatchException(method);

            ShouldNotBeNull(exception);
            ShouldBeAssignableTo(exception, exceptionType);

            return exception;
        }

        private static Exception CatchException(Action throwingAction)
        {
            try
            {
                throwingAction();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        public static void ShouldBeLike(this object obj, object expected)
        {
            var exceptions = ShouldBeLikeInternal(obj, expected, string.Empty, new HashSet<ReferentialEqualityTuple>()).ToArray();

            if (exceptions.Any())
            {
                throw NewException(exceptions.Select(e => e.Message).Aggregate(string.Empty, (r, m) => r + m + Environment.NewLine + Environment.NewLine).TrimEnd());
            }
        }

        private static IEnumerable<SpecificationException> ShouldBeLikeInternal(object obj, object expected, string nodeName, HashSet<ReferentialEqualityTuple> visited)
        {
            // Stop at already checked <actual,expected>-pairs to prevent infinite loops (cycles in object graphs). Additionally
            // this also avoids re-equality-evaluation for already compared pairs.
            var objExpectedTuple = new ReferentialEqualityTuple(obj, expected);

            if (visited.Contains(objExpectedTuple))
            {
                return Enumerable.Empty<SpecificationException>();
            }

            visited.Add(objExpectedTuple);

            ObjectGraphHelper.INode expectedNode = null;

            var nodeType = typeof(ObjectGraphHelper.LiteralNode);

            if (obj != null && expected != null)
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
                    return new[] { NewException($"{{0}}:{Environment.NewLine}{ex.Message}", nodeName) };
                }

                return Enumerable.Empty<SpecificationException>();
            }

            if (nodeType == typeof(ObjectGraphHelper.SequenceNode))
            {
                if (obj == null)
                {
                    var errorMessage = PrettyPrintingExtensions.FormatErrorMessage(null, expected);

                    return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var actualNode = ObjectGraphHelper.GetGraph(obj);

                if (actualNode.GetType() != typeof(ObjectGraphHelper.SequenceNode))
                {
                    var errorMessage = $"  Expected: Array or Sequence{Environment.NewLine}  But was:  {obj.GetType()}";

                    return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var expectedValues = ((ObjectGraphHelper.SequenceNode)expectedNode)?.ValueGetters.ToArray();
                var actualValues = ((ObjectGraphHelper.SequenceNode)actualNode).ValueGetters.ToArray();

                var expectedCount = expectedValues?.Length ?? 0;
                var actualCount = actualValues.Length;

                if (expectedCount != actualCount)
                {
                    var errorMessage = string.Format("  Expected: Sequence length of {1}{0}  But was:  {2}", Environment.NewLine, expectedCount, actualCount);

                    return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                return Enumerable.Range(0, expectedCount)
                    .SelectMany(i => ShouldBeLikeInternal(actualValues.ElementAt(i)(), expectedValues?.ElementAt(i)(), $"{nodeName}[{i}]", visited));
            }

            if (nodeType == typeof(ObjectGraphHelper.KeyValueNode))
            {
                var actualNode = ObjectGraphHelper.GetGraph(obj);

                if (actualNode.GetType() != typeof(ObjectGraphHelper.KeyValueNode))
                {
                    var errorMessage = $"  Expected: Class{Environment.NewLine}  But was:  {obj?.GetType()}";

                    return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var expectedKeyValues = ((ObjectGraphHelper.KeyValueNode)expectedNode)?.KeyValues;
                var actualKeyValues = ((ObjectGraphHelper.KeyValueNode)actualNode).KeyValues;

                return expectedKeyValues?.SelectMany(kv =>
                {
                    var fullNodeName = string.IsNullOrEmpty(nodeName)
                        ? kv.Name
                        : $"{nodeName}.{kv.Name}";
                    var actualKeyValue = actualKeyValues.SingleOrDefault(k => k.Name == kv.Name);

                    if (actualKeyValue == null)
                    {
                        var errorMessage = string.Format("  Expected: {1}{0}  But was:  Not Defined",
                            Environment.NewLine, kv.ValueGetter().ToUsefulString());

                        return new[] {NewException($"{{0}}:{Environment.NewLine}{errorMessage}", fullNodeName)};
                    }

                    return ShouldBeLikeInternal(actualKeyValue.ValueGetter(), kv.ValueGetter(), fullNodeName, visited);
                });
            }

            throw new InvalidOperationException("Unknown node type");
        }

        private class ReferentialEqualityTuple
        {
            private readonly object obj;

            private readonly object expected;

            public ReferentialEqualityTuple(object obj, object expected)
            {
                this.obj = obj;
                this.expected = expected;
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode (obj) * RuntimeHelpers.GetHashCode (expected);
            }

            public override bool Equals(object other)
            {
                if (!(other is ReferentialEqualityTuple otherSimpleTuple))
                {
                    return false;
                }

                return ReferenceEquals(obj, otherSimpleTuple.obj) && ReferenceEquals(expected, otherSimpleTuple.expected);
            }
        }
    }
}
