using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Machine.Specifications.Formatting;
using Machine.Specifications.Reflection;

namespace Machine.Specifications
{
    public static class EquivalenceExtensions
    {
        public static void ShouldBeLike(this object value, object expected)
        {
            var exceptions = ShouldBeLike(value, expected, string.Empty, new HashSet<ReferentialEqualityTuple>()).ToArray();

            if (exceptions.Any())
            {
                var message = exceptions
                    .Select(e => e.Message)
                    .Aggregate(string.Empty, (r, m) => r + m + Environment.NewLine + Environment.NewLine)
                    .TrimEnd();

                throw Exceptions.Specification(message);
            }
        }

        private static IEnumerable<SpecificationException> ShouldBeLike(object? value, object? expected, string nodeName, ISet<ReferentialEqualityTuple> visited)
        {
            // Stop at already checked <actual,expected>-pairs to prevent infinite loops (cycles in object graphs). Additionally
            // this also avoids re-equality-evaluation for already compared pairs.
            var tuple = new ReferentialEqualityTuple(value, expected);

            if (visited.Contains(tuple))
            {
                return Enumerable.Empty<SpecificationException>();
            }

            visited.Add(tuple);

            var expectedNode = default(INode);
            var nodeType = typeof(LiteralNode);

            if (value != null && expected != null)
            {
                expectedNode = ObjectGraph.Get(expected);
                nodeType = expectedNode.GetType();
            }

            if (nodeType == typeof(LiteralNode))
            {
                try
                {
                    value.ShouldEqual(expected);
                }
                catch (SpecificationException ex)
                {
                    return new[] { Exceptions.Specification($"{{0}}:{Environment.NewLine}{ex.Message}", nodeName) };
                }

                return Enumerable.Empty<SpecificationException>();
            }

            if (nodeType == typeof(SequenceNode))
            {
                if (value == null)
                {
                    var errorMessage = ObjectExtensions.FormatErrorMessage(null, expected);

                    return new[] { Exceptions.Specification($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var actualNode = ObjectGraph.Get(value);

                if (actualNode.GetType() != typeof(SequenceNode))
                {
                    var errorMessage = $"  Expected: Array or Sequence{Environment.NewLine}  But was:  {value.GetType()}";

                    return new[] { Exceptions.Specification($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var expectedValues = ((SequenceNode) expectedNode!).ValueGetters.ToArray();
                var actualValues = ((SequenceNode) actualNode).ValueGetters.ToArray();

                var expectedCount = expectedValues?.Length ?? 0;
                var actualCount = actualValues.Length;

                if (expectedCount != actualCount)
                {
                    var errorMessage = string.Format("  Expected: Sequence length of {1}{0}  But was:  {2}", Environment.NewLine, expectedCount, actualCount);

                    return new[] { Exceptions.Specification($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                return Enumerable.Range(0, expectedCount)
                    .SelectMany(i => ShouldBeLike(actualValues.ElementAt(i)(), expectedValues?.ElementAt(i)(), $"{nodeName}[{i}]", visited));
            }

            if (nodeType == typeof(KeyValueNode))
            {
                var actualNode = ObjectGraph.Get(value!);

                if (actualNode.GetType() != typeof(KeyValueNode))
                {
                    var errorMessage = $"  Expected: Class{Environment.NewLine}  But was:  {value?.GetType()}";

                    return new[] { Exceptions.Specification($"{{0}}:{Environment.NewLine}{errorMessage}", nodeName) };
                }

                var expectedKeyValues = ((KeyValueNode) expectedNode!).KeyValues;
                var actualKeyValues = ((KeyValueNode) actualNode).KeyValues;

                return expectedKeyValues
                    .SelectMany(kv =>
                    {
                        var fullNodeName = string.IsNullOrEmpty(nodeName)
                            ? kv.Name
                            : $"{nodeName}.{kv.Name}";
                        var actualKeyValue = actualKeyValues.SingleOrDefault(k => k.Name == kv.Name);

                        if (actualKeyValue == null)
                        {
                            var errorMessage = string.Format("  Expected: {1}{0}  But was:  Not Defined", Environment.NewLine, kv.ValueGetter().Format());

                            return new[] {Exceptions.Specification($"{{0}}:{Environment.NewLine}{errorMessage}", fullNodeName)};
                        }

                        return ShouldBeLike(actualKeyValue.ValueGetter(), kv.ValueGetter(), fullNodeName, visited);
                    });
            }

            throw new InvalidOperationException("Unknown node type");
        }

        private class ReferentialEqualityTuple
        {
            private readonly object? value;

            private readonly object? expected;

            public ReferentialEqualityTuple(object? value, object? expected)
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
                if (other is not ReferentialEqualityTuple otherTuple)
                {
                    return false;
                }

                return ReferenceEquals(value, otherTuple.value) &&
                       ReferenceEquals(expected, otherTuple.expected);
            }
        }
    }
}
