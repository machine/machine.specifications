using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Machine.Specifications.Runner.Utility
{
    public static class Naming
    {
        private static readonly Regex QuoteRegex = new Regex(@"(?<quoted>__(?<inner>\w+?)__)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string ToFormat(this Type type)
        {
            while (true)
            {
                var args = type.GetGenericArguments();

                if (args.Length == 0 || args.Any(x => x.IsGenericParameter))
                {
                    break;
                }

                type = type.GetGenericTypeDefinition();
            }

            var typeName = type.Name;
            var index = typeName.IndexOf('`');

            if (index > 0)
            {
                typeName = typeName.Substring(0, index);
            }

            if (typeName.Length > 0)
            {
                typeName = char.ToLower(typeName[0]) + typeName.Substring(1);
            }

            return ToFormat(typeName);
        }

        public static string ToFormat(this string name)
        {
            name = ReplaceDoubleUnderscoresWithQuotes(name);
            name = ReplaceUnderscoreEssWithPossessive(name);
            name = ReplaceSingleUnderscoresWithSpaces(name);

            return name;
        }

        static string ReplaceUnderscoreEssWithPossessive(string specificationName)
        {
            return specificationName.Replace("_s_", "'s ");
        }

        static string ReplaceSingleUnderscoresWithSpaces(string specificationName)
        {
            return specificationName.Replace("_", " ");
        }

        static string ReplaceDoubleUnderscoresWithQuotes(string specificationName)
        {
            return QuoteRegex.Replace(specificationName, " \"${inner}\" ");
        }
    }
}
