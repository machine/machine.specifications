using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Machine.Specifications
{
    internal static class StringExtensions
    {
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
    }
}
