using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;

namespace Machine.Specifications.ConsoleRunner.Specs
{
  public class Running_with_no_command_line_arguments
  {
    static Runner runner;
    static IConsole console;
    Context before_each =()=>
    {
      MockRepository mocks = new MockRepository();
      console = mocks.ToBeNamedMock<IConsole>();
      runner = new Runner(console);
    };

    When run =()=>
      runner.Run(new string[] {});

    It should_print_usage_statement =()=>
      console.Verify(x => x.WriteLine(Arg<string>.Matches(y => true)));
  }

  public class Running_with_assemblies_in_the_command_line
  {
    static Runner runner;
    static IConsole console;
    Context before_each =()=>
    {
      MockRepository mocks = new MockRepository();
      console = mocks.ToBeNamedMock<IConsole>();
      runner = new Runner(console);
    };

    When run =()=>
      runner.Run(new string[] {"Machine.Specifications.Example.dll"});

    It should_print_the_results =()=>
      console.Verify(x => x.WriteLine(Arg<string>.Matches(y => y.IndexOf("It should") >= 0)), c => c.Repeat.Times(5));
  }
}
