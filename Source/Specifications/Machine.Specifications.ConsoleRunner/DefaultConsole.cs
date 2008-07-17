using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.ConsoleRunner
{
  public class DefaultConsole : IConsole
  {
    public void WriteLine(string line)
    {
      Console.WriteLine(line);
    }

    public void WriteLine(string line, params object[] parameters)
    {
      Console.WriteLine(String.Format(line, parameters));
    }
  }
}
