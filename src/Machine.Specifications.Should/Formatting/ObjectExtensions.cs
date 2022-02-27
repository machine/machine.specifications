using System;
using System.Collections;
using System.Linq;

namespace Machine.Specifications.Formatting
{
    internal static class ObjectExtensions
    {
        public static string Format(this object? value)
        {
            if (value == null)
            {
                return "[null]";
            }

            if (value is string valueAsString)
            {
                return $@"""{valueAsString.Replace("\n", "\\n")}""";
            }

            var type = value.GetType();

            if (type.IsValueType)
            {
                return "[" + value + "]";
            }

            if (value is IEnumerable items)
            {
                var enumerable = items.Cast<object>();

                return type + ":\r\n" + enumerable.EachToUsefulString();
            }

            var stringValue = value.ToString();

            if (stringValue == null || stringValue.Trim() == string.Empty)
            {
                return $"{type}:[]";
            }

            stringValue = stringValue.Trim();

            if (stringValue.Contains("\n"))
            {
                return string.Format(@"{1}:
[
{0}
]", stringValue.Indent(), type);
            }

            var typeString = type.ToString();

            if (typeString == stringValue)
            {
                return typeString;
            }

            return $"{type}:[{stringValue}]";
        }

        public static string FormatErrorMessage(this object? actualObject, object? expectedObject)
        {
            if (actualObject is string actual && expectedObject is string expected)
            {
                var message = GetExpectedStringLengthMessage(expected.Length, actual.Length);
                var index = IndexOf(actual, expected);

                GetStringsAroundFirstDifference(expected, actual, index, out var actualReported, out var expectedReported);

                var count = IndexOf(actualReported, expectedReported);

                return string.Format(
                    "  {1} Strings differ at index {2}.{0}" +
                    "  Expected: \"{3}\"{0}" +
                    "  But was:  \"{4}\"{0}" +
                    "  -----------{5}^",
                    Environment.NewLine,
                    message,
                    index,
                    expectedReported,
                    actualReported,
                    new string('-', count));
            }

            var actualValue = actualObject.Format();
            var expectedValue = expectedObject.Format();

            return string.Format("  Expected: {1}{0}  But was:  {2}", Environment.NewLine, expectedValue, actualValue);
        }

        private static void GetStringsAroundFirstDifference(string expected, string actual, int index, out string actualReported, out string expectedReported)
        {
            var left = index;
            var actualRight = index;
            var expectedRight = index;
            var keepAugmenting = true;

            while (keepAugmenting && IsInCopyFrameLength(left, actualRight, actual.Length) && IsInCopyFrameLength(left, expectedRight, expected.Length))
            {
                keepAugmenting = false;

                if (left > 0)
                {
                    left--;
                    keepAugmenting = true;
                }

                if (IsInCopyFrameLength(left, actualRight, actual.Length) && IsInCopyFrameLength(left, expectedRight, expected.Length))
                {
                    if (actual.Length > actualRight)
                    {
                        actualRight++;
                        keepAugmenting = true;
                    }

                    if (expected.Length > expectedRight)
                    {
                        expectedRight++;
                        keepAugmenting = true;
                    }
                }
            }

            actualReported = actual.Substring(left, actualRight - left);
            expectedReported = expected.Substring(left, expectedRight - left);

            if (left != 0)
            {
                actualReported = "..." + actualReported;
                expectedReported = "..." + expectedReported;
            }

            if (actualRight != actual.Length || expectedRight != expected.Length)
            {
                actualReported += "...";
                expectedReported += "...";
            }
        }

        private static bool IsInCopyFrameLength(int start, int end, int max)
        {
            var length = end - start;

            if (start > 0)
            {
                length += 3;
            }

            if (end < max)
            {
                length += 3;
            }

            return length < 64;
        }

        private static int IndexOf(string actual, string expected)
        {
            for (var i = 0; i < actual.Length; i++)
            {
                if (expected.Length <= i || expected[i] != actual[i])
                {
                    return i;
                }
            }

            return actual.Length;
        }

        private static string GetExpectedStringLengthMessage(int actual, int expected)
        {
            return actual == expected
                ? $"String lengths are both {actual}."
                : $"Expected string length {actual} but was {expected}.";
        }
    }
}
