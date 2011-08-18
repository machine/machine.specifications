﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class DefaultConsole : IConsole
  {
    public void Write(string line)
    {
      Console.Write(line);
    }

    public void Write(string line, params object[] parameters)
    {
      Console.Write(String.Format(line, parameters));
    }

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
