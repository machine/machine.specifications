using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.ConsoleRunner
{
  public interface IConsole
  {
    void WriteLine(string line);
    void WriteLine(string line, params string[] parameters);
  }
}