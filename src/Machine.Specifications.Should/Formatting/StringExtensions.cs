using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Machine.Specifications.Formatting
{
    internal static class StringExtensions
    {
        private const string EscapedBraces = "{{{0}}}";

        private static readonly Regex FormatBraces = new Regex(@"{([^\d].*?)}", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly string[] Delimiters =
        {
            "\r\n",
            "\n"
        };

        public static string Indent(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var parts = value.Split(Delimiters, StringSplitOptions.None);
            var result = new StringBuilder($"  {parts[0]}");

            foreach (var part in parts.Skip(1))
            {
                result.AppendLine();
                result.Append("  " + part);
            }

            return result.ToString();
        }

        public static string EnsureSafeFormat(this string message)
        {
            return FormatBraces.Replace(message, match => string.Format(EscapedBraces, match.Groups[0]));
        }
    }
}
