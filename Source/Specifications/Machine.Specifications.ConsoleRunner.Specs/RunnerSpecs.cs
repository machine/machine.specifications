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
      console = mocks.DynamicMock<IConsole>();
      runner = new Runner(console);
    };

    Because run =()=>
      runner.Run(new string[] {});

    It should_print_usage_statement =()=>
      console.AssertWasCalled(x => x.WriteLine(Arg<string>.Matches(y => true)));
  }

  public class Running_with_assemblies_in_the_command_line
  {
    static Runner runner;
    static IConsole console;
    Context before_each =()=>
    {
      MockRepository mocks = new MockRepository();
      console = mocks.DynamicMock<IConsole>();
      runner = new Runner(console);
    };

    Because run =()=>
      runner.Run(new string[] {"Machine.Specifications.Example.dll"});

    It should_print_the_results =()=>
      console.AssertWasCalled(x => x.WriteLine(Arg<string>.Matches(y => y.IndexOf("It should") >= 0)), c => c.Repeat.Times(5));
  }
}
