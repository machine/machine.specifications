using System.Linq;
using System.Text.RegularExpressions;
using Machine.Specifications.Formatting;

namespace Machine.Specifications
{
    internal static class Exceptions
    {
        private const string EscapedBraces = "{{{0}}}";

        private static readonly Regex FormatBraces = new(@"{([^\d].*?)}", RegexOptions.Compiled | RegexOptions.Singleline);

        public static SpecificationException Specification(string message, params object[] parameters)
        {
            if (parameters.Any())
            {
                return new SpecificationException(string.Format(EnsureSafeFormat(message), parameters.Select(x => x.Format()).Cast<object>().ToArray()));
            }

            return new SpecificationException(message);
        }

        private static string EnsureSafeFormat(string message)
        {
            return FormatBraces.Replace(message, match => string.Format(EscapedBraces, match.Groups[0]));
        }
    }
}
