using System;
using System.Collections.Generic;

namespace CodeWeaving.Matcher.SampleCode
{
  public class SuperDuperAttribute : Attribute
  {
  }

  public interface IHasStuff
  {
    [SuperDuper]
    IEnumerable<string> Stuff
    {
      get;
    }
    List<string> RealStuff
    {
      get;
    }
  }

  public class HasStuff : IHasStuff
  {
    #region Member Data
    private readonly List<string> stuff = new List<string>();
    #endregion

    #region IHasStuff Members
    public IEnumerable<string> Stuff
    {
      get { return stuff; }
    }
    public List<string> RealStuff
    {
      get { return stuff; }
    }
    #endregion
  }

  public class SomethingElse
  {
    public void DoOtherStuff()
    {
      IHasStuff whatever = new HasStuff();
      foreach (string name in whatever.Stuff)
      {
        Console.Write(name);
      }
    }

    public void DoStuff()
    {
      IHasStuff whatever = new HasStuff();
      foreach (string name in whatever.RealStuff)
      {
        Console.Write(name);
      }
    }
  }
}