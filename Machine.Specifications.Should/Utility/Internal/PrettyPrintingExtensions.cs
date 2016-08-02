using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;


namespace Machine.Specifications.Utility.Internal
{
    public static class PrettyPrintingExtensions
    {
        private const string CurlyBraceSurround = "{{{0}}}";
        private static readonly Regex EscapeNonFormatBraces = new Regex(@"{([^\d].*?)}", RegexOptions.Compiled | RegexOptions.Singleline);

        public static string FormatErrorMessage(object actualObject, object expectedObject)
        {
            if (actualObject == null || !(actualObject is string) ||
                expectedObject == null || !(expectedObject is string))
            {
                // format objects
                var actual = actualObject.ToUsefulString();
                var expected = expectedObject.ToUsefulString();
                return string.Format("  Expected: {1}{0}  But was:  {2}", Environment.NewLine, expected, actual);
            }
            else
            {
                // format strings
                var actual = actualObject as string;
                var expected = expectedObject as string;

                var expectedStringLengthMessage = GetExpectedStringLengthMessage(expected.Length, actual.Length);
                var firstIndexOfFirstDifference = GetFirstIndexOfFirstDifference(actual, expected);
                string actualReported;
                string expectedReported;
                GetStringsAroundFirstDifference(expected, actual, firstIndexOfFirstDifference, out actualReported, out expectedReported);

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
        }

        static void GetStringsAroundFirstDifference(string expected, string actual, int firstIndexOfFirstDifference, out string actualReported, out string expectedReported)
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

        static bool IsInCopyFrameLength(int start, int end, int max)
        {
            int length = end - start;
            if (start > 0)
                length += 3;
            if (end < max)
                length += 3;
            return length < 64;
        }

        static int GetFirstIndexOfFirstDifference(string actual, string expected)
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

        static string GetExpectedStringLengthMessage(int actual, int expected)
        {
            if (actual == expected)
            {
                return string.Format("String lengths are both {0}.", actual);
            }

            return string.Format("Expected string length {0} but was {1}.", actual, expected);
        }

        static string Tab(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            var split = str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();

            sb.Append("  " + split[0]);
            foreach (var part in split.Skip(1))
            {
                sb.AppendLine();
                sb.Append("  " + part);
            }

            return sb.ToString();
        }

        public static string EachToUsefulString<T>(this IEnumerable<T> enumerable)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.Append(String.Join(",\r\n", enumerable.Select(x => x.ToUsefulString().Tab()).Take(10).ToArray()));
            if (enumerable.Count() > 10)
            {
                if (enumerable.Count() > 11)
                {
                    sb.AppendLine(String.Format(",\r\n  ...({0} more elements)", enumerable.Count() - 10));
                }
                else
                {
                    sb.AppendLine(",\r\n" + enumerable.Last().ToUsefulString().Tab());
                }
            }
            else
            {
                sb.AppendLine();
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        internal static string ToUsefulString(this object obj)
        {
            string str;
            if (obj == null)
            {
                return "[null]";
            }
            if (obj.GetType() == typeof(string))
            {
                str = (string)obj;

                return "\"" + str.Replace("\n", "\\n") + "\"";
            }
            if (obj.GetType().GetTypeInfo().IsValueType)
            {
                return "[" + obj + "]";
            }

            if (obj is IEnumerable)
            {
                var enumerable = ((IEnumerable)obj).Cast<object>();

                return obj.GetType() + ":\r\n" + enumerable.EachToUsefulString();
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
            {
                return obj.GetType().ToString();
            }

            return string.Format("{0}:[{1}]", obj.GetType(), str);
        }

        internal static string EnsureSafeFormat(this string message)
        {
            return EscapeNonFormatBraces.Replace(message, match => string.Format(CurlyBraceSurround, match.Groups[0]));
        }
    }
}