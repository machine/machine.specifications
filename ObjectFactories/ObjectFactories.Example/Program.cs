using System;
using System.Collections.Generic;

namespace ObjectFactories.Example
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      EmailSender emailer1 = new EmailSender();
      emailer1.TellEveryone("Hello, World!");
      emailer1.TellEveryone("Bye, World");

      EmailSender emailer2 = new EmailSender();
      emailer2.TellEveryone("Hello, World!");

      IHasStuff stuff = new HasStuff();
      for (int i = 0; i < 1000000; ++i)
      {
        foreach (string value in stuff.Values)
        {
          if (value == null)
          {
            throw new ArgumentException();
          }
        }
      }
    }
  }
  public interface IHasStuff
  {
    IEnumerable<string> Values
    {
      get;
    }
  }
  public class HasStuff : IHasStuff
  {
    #region Member Data
    private readonly List<string> _values = new List<string>();
    #endregion

    #region IHasStuff Members
    public IEnumerable<string> Values
    {
      get { return _values; }
    }
    #endregion

    #region HasStuff()
    public HasStuff()
    {
      _values.Add("Jacob");
      _values.Add("Andy");
    }
    #endregion
  }
}
