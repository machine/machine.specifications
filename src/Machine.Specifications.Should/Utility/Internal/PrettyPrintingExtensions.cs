using System;
using Machine.Specifications.Formatting;

namespace Machine.Specifications.Utility.Internal
{
    internal static class PrettyPrintingExtensions
    {
        public static string FormatErrorMessage(object actualObject, object expectedObject)
        {
            if (actualObject is string && expectedObject is string)
            {
                var actual = actualObject as string;
                var expected = expectedObject as string;

                var expectedStringLengthMessage = GetExpectedStringLengthMessage(expected.Length, actual.Length);
                var firstIndexOfFirstDifference = GetFirstIndexOfFirstDifference(actual, expected);

                GetStringsAroundFirstDifference(expected, actual, firstIndexOfFirstDifference, out var actualReported, out var expectedReported);

                var count = GetFirstIndexOfFirstDifference(actualReported, expectedReported);

                return string.Format(
                          "  {1} Strings differ at index {2}.{0}" +
                          "  Expected: \"{3}\"{0}" +
                          "  But was:  \"{4}\"{0}" +
                          "  -----------{5}^",
                          Environment.NewLine,
                          expectedStringLengthMessage,
                          firstIndexOfFirstDifference,
                          expectedReported,
                          actualReported,
                          new string('-', count));
            }

            var actualValue = actualObject.ToUsefulString();
            var expectedValue = expectedObject.ToUsefulString();

            return string.Format("  Expected: {1}{0}  But was:  {2}", Environment.NewLine, expectedValue, actualValue);
        }

        private static void GetStringsAroundFirstDifference(string expected, string actual, int firstIndexOfFirstDifference, out string actualReported, out string expectedReported)
        {
            var left = firstIndexOfFirstDifference;
            var actualRight = firstIndexOfFirstDifference;
            var expectedRight = firstIndexOfFirstDifference;
            var keepAugmenting = true;

            while (keepAugmenting &&
                IsInCopyFrameLength(left, actualRight, actual.Length) &&
                IsInCopyFrameLength(left, expectedRight, expected.Length))
            {
                keepAugmenting = false;

                if (left > 0)
                {
                    left--;
                    keepAugmenting = true;
                }

                if (IsInCopyFrameLength(left, actualRight, actual.Length) &&
                    IsInCopyFrameLength(left, expectedRight, expected.Length))
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
                actualReported = actualReported + "...";
                expectedReported = expectedReported + "...";
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

        private static int GetFirstIndexOfFirstDifference(string actual, string expected)
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
            if (actual == expected)
            {
                return $"String lengths are both {actual}.";
            }

            return $"Expected string length {actual} but was {expected}.";
        }
    }
}
