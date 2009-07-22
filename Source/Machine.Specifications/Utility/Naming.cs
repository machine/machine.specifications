using System.Text.RegularExpressions;

namespace Machine.Specifications.Utility
{
  public static class Naming
  {
    static readonly Regex QuoteRegex = new Regex(@"(?<quoted>__(?<inner>\w+?)__)",
                                                 RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string ReplaceUnderscores(this string name)
    {
      name = ReplaceDoubleUnderscoresWithQuotes(name);
      name = ReplaceUnderscoreEssWithPossessive(name);
      name = ReplaceSingleUnderscoresWithSpaces(name);

      return name;
    }

    public static string ReplaceSpaces(this string name)
    {
      return name.Replace(' ', '_');
    }

    static string ReplaceUnderscoreEssWithPossessive(string specificationName)
    {
      specificationName = specificationName.Replace("_s_", "'s ");
      return specificationName;
    }

    static string ReplaceSingleUnderscoresWithSpaces(string specificationName)
    {
      specificationName = specificationName.Replace("_", " ");
      return specificationName;
    }

    static string ReplaceDoubleUnderscoresWithQuotes(string specificationName)
    {
      specificationName = QuoteRegex.Replace(specificationName, " \"${inner}\" ");

      return specificationName;
    }
  }
}