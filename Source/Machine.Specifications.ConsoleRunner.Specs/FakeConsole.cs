using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.ConsoleRunner.Specs
{
  public class FakeConsole : IConsole
  {
    readonly StringBuilder _console = new StringBuilder();

    public List<string> Lines
    {
      get { return _console.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList(); }
    }

    #region IConsole Members
    public void Write(string line)
    {
      _console.Append(line);
    }

    public void Write(string line, params object[] parameters)
    {
      _console.AppendFormat(line, parameters);
    }

    public void WriteLine(string line)
    {
      _console.AppendLine(line);
    }

    public void WriteLine(string line, params object[] parameters)
    {
      _console.AppendLine(string.Format(line, parameters));
    }
    #endregion

    public void ShouldContainLineWith(string s)
    {
      Lines.Where(x => x.Contains(s)).Any().ShouldBeTrue();
    }

    public void ShouldNotContainLineWith(string s)
    {
      Lines.Where(x => x.Contains(s)).Any().ShouldBeFalse();
    }
  }
}