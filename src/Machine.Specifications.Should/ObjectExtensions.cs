using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Machine.Specifications.Reflection;
using Machine.Specifications.Text;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    public static class ObjectExtensions
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

        private static SpecificationException NewException(string message, params object[] parameters)
        {
            if (parameters.Any())
            {
                return new SpecificationException(string.Format(message.EnsureSafeFormat(), parameters.Select(x => x.ToUsefulString()).Cast<object>().ToArray()));
            }

            return new SpecificationException(message);
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

            var expectedNode = default(INode);

            var nodeType = typeof(LiteralNode);

            if (obj != null && expected != null)
            {
                expectedNode = ObjectGraph.Get(expected);
                nodeType = expectedNode.GetType();
            }

            if (nodeType == typeof(LiteralNode))
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

            if (nodeType == typeof(SequenceNode))
            {
                if (obj == null)
                {
                    var errorMessage = PrettyPrintingExtensions.FormatErrorMessage(null, expected);

                    return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var actualNode = ObjectGraph.Get(obj);

                if (actualNode.GetType() != typeof(SequenceNode))
                {
                    var errorMessage = $"  Expected: Array or Sequence{Environment.NewLine}  But was:  {obj.GetType()}";

                    return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var expectedValues = ((SequenceNode) expectedNode)?.ValueGetters.ToArray();
                var actualValues = ((SequenceNode) actualNode).ValueGetters.ToArray();

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

            if (nodeType == typeof(KeyValueNode))
            {
                var actualNode = ObjectGraph.Get(obj);

                if (actualNode.GetType() != typeof(KeyValueNode))
                {
                    var errorMessage = $"  Expected: Class{Environment.NewLine}  But was:  {obj?.GetType()}";

                    return new[] { NewException($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var expectedKeyValues = ((KeyValueNode) expectedNode)?.KeyValues;
                var actualKeyValues = ((KeyValueNode) actualNode).KeyValues;

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
            private readonly object value;

            private readonly object expected;

            public ReferentialEqualityTuple(object value, object expected)
            {
                this.value = value;
                this.expected = expected;
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(value) * RuntimeHelpers.GetHashCode(expected);
            }

            public override bool Equals(object other)
            {
                if (!(other is ReferentialEqualityTuple otherSimpleTuple))
                {
                    return false;
                }

                return ReferenceEquals(value, otherSimpleTuple.value) && ReferenceEquals(expected, otherSimpleTuple.expected);
            }
        }
    }
}
