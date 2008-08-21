using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using Machine.Specifications.ConsoleRunner.Properties;

namespace Machine.Specifications.ConsoleRunner
{
  public class Options
  {

    [Option(null,
      "html",
      HelpText = "Ouputs HTML (one-per-assembly) at the specified path.")] 
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
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Machine.Specifications");
      sb.AppendLine("Copyright (C) 2007, 2008");
      sb.AppendLine("");
      sb.AppendLine(Resources.UsageStatement);
      sb.AppendLine("Options:");
      sb.AppendLine("  -s, --silent      Suppress console output");
      sb.AppendLine("  --html <PATH>     Outputs an HTML file(s) to path, one-per-assembly w/ index.html (if directory, otherwise all are in one file)");
      sb.AppendLine("  -h, --help        Shows this help message");


      return sb.ToString();
    }

    public virtual bool ParseArguments(string[] args)
    {
      return Parser.ParseArguments(args, this, Console.Out);
    }

  }
}
