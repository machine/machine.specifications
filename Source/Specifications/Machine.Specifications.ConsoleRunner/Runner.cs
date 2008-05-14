using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.ConsoleRunner.Properties;

namespace Machine.Specifications.ConsoleRunner
{
  public class Runner
  {
    readonly IConsole _console;

    public Runner(IConsole console)
    {
      _console = console;
    }

    public void Run(string[] arguments)
    {
      _console.WriteLine(Resources.UsageStatement);
    }
  }
}
