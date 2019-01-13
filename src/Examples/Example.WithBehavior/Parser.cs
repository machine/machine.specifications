using System;

namespace Example.WithBehavior
{
  public interface IParser
  {
    DateTime Parse(string date);
  }

  internal class RegexParser : IParser
  {
    #region IParser Members
    public DateTime Parse(string date)
    {
      // Parse with a regular expression. Not that it's recommended, but that's why this example is contrived.
      return new DateTime(2009, 1, 21);
    }
    #endregion
  }

  internal class InfrastructureParser : IParser
  {
    #region IParser Members
    public DateTime Parse(string date)
    {
      // Parse with DateTime.Parse.
      return new DateTime(2009, 1, 21);
    }
    #endregion
  }
}