using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Machine.Specifications.Utility;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    internal static class ObjectExtensions
    {
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

                var expectedValues = ((ObjectGraphHelper.SequenceNode) expectedNode)?.ValueGetters.ToArray();
                var actualValues = ((ObjectGraphHelper.SequenceNode) actualNode).ValueGetters.ToArray();

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

                var expectedKeyValues = ((ObjectGraphHelper.KeyValueNode) expectedNode)?.KeyValues;
                var actualKeyValues = ((ObjectGraphHelper.KeyValueNode) actualNode).KeyValues;

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

                        return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", fullNodeName) };
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
                return RuntimeHelpers.GetHashCode(obj) * RuntimeHelpers.GetHashCode(expected);
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
