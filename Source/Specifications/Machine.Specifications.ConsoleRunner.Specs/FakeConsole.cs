using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.ConsoleRunner.Specs
{
  public class FakeConsole : IConsole
  {
    readonly List<string> _lines = new List<string>();

    public List<string> Lines
    {
      get { return _lines; }
    }

    public void Write(string line)
    {
      _lines.Add(line);
    }

    public void Write(string line, params object[] parameters)
    {
      _lines.Add(string.Format(line, parameters));
    }

    public void WriteLine(string line)
    {
      _lines.Add(line);
    }

    public void WriteLine(string line, params object[] parameters)
    {
      _lines.Add(string.Format(line, parameters));
    }

    public void ShouldContainLineWith(string s)
    {
      _lines.Where(x=>x.Contains(s)).Count().ShouldBeGreaterThan(0);
    }
  }
}