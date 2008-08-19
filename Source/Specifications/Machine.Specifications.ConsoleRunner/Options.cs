using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Machine.Specifications.ConsoleRunner
{
  public class Options
  {

    [Option(null,
      "html",
      HelpText = "Path to output HTML file(s). Formatted as: AssemblyName-MMDDYYYY-HHMM.html. " + 
      "Will create an index-MMDDYYYY-HHMM.html in the case of multiple assemblies.")] 
    public string HtmlPath = string.Empty;

    [Option("s",
      "silent",
      HelpText = "Suppresses all console output.")]
    public bool Silent = false;

    [ValueList(typeof(List<string>))]
    public IList<string> assemblyFiles = null;

    [HelpOption]
    public string GetUsage()
    {
      HelpText info = new HelpText("Machine.Specifications");
      info.AddOptions(this);
      return info;
    }

    public virtual bool ParseArguments(string[] args)
    {
      return Parser.ParseArguments(args, this, Console.Out);
    }

  }
}
