using System.Collections.Generic;

namespace Machine.Specifications.ConsoleRunner.Specs
{
  public class FakeConsole : IConsole
  {
    readonly List<string> _lines = new List<string>();

    public List<string> Lines
    {
      get { return _lines; }
    }

    public void WriteLine(string line)
    {
      _lines.Add(line);
    }

    public void WriteLine(string line, params string[] parameters)
    {
      _lines.Add(string.Format(line, parameters));
    }
  }
}