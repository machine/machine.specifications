using System.Text.RegularExpressions;

namespace Machine.Specifications.ConsoleRunner.Specs
{
    public static class ShouldExtensions
    {
        public static void ShouldMatchRegex(this string value, string pattern)
        {
            if (!Regex.IsMatch(value, pattern))
                throw new SpecificationException($"Regex is not a match for: {value}");
        }
    }
}
