using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class Options
  {
    [Option("xml", "xml", HelpText = "Outputs XML")]
    public string XmlPath = string.Empty;

    [Option(null,
      "html",
      HelpText = "Ouputs HTML (one-per-assembly) at the specified path.")] 
    public string HtmlPath = string.Empty;

    [Option("s",
      "silent",
      HelpText = "Suppresses all console output.")]
    public bool Silent = false;

    [Option("t",
      "timeinfo",
      HelpText = "Adds time-related information in HTML output")] 
    public bool ShowTimeInformation = false;

    [Option(null, 
      "teamcity", 
      HelpText = "Replaces the standard build reporting with TeamCity test reporting.")]
    public bool TeamCityIntegration = false;

    [OptionList("i",
      "include",
      HelpText = "Executes all specifications in contexts with these comma delimited tags. Ex. -i \"foo,bar,foo_bar\"",
      Separator = ',')] 
    public IList<string> IncludeTags = null;

    [OptionList("x",
      "exclude",
      HelpText = "Exclude specifications in contexts with these comma delimited tags. Ex. -x \"foo,bar,foo_bar\"",
      Separator = ',')] 
    public IList<string> ExcludeTags = null;

    [ValueList(typeof(List<string>))]
    public IList<string> AssemblyFiles = null;

    [HelpOption]
    public string GetUsage()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Machine.Specifications");
      sb.AppendLine("Copyright (C) 2007, 2008");
      sb.AppendLine("");
      sb.AppendLine(Resources.UsageStatement);
      sb.AppendLine("Options:");
      sb.AppendLine("  -i, --include     Executes all specifications in contexts with these comma delimited tags. Ex. -i \"foo,bar,foo_bar\"");
      sb.AppendLine("  -x, --exclude     Exclude specifications in contexts with these comma delimited tags. Ex. -x \"foo,bar,foo_bar\"");
      sb.AppendLine("  -t, --timeinfo    Shows time-related information in HTML output");
      sb.AppendLine("  -s, --silent      Suppress console output");
      sb.AppendLine("  --teamcity        Reporting for TeamCity CI integration.");
      sb.AppendLine("  --html <PATH>     Outputs an HTML file(s) to path, one-per-assembly w/ index.html (if directory, otherwise all are in one file)");
      sb.AppendLine("  --xml <PATH>      Outputs an XML file(s) to path");
      sb.AppendLine("  -h, --help        Shows this help message");

      return sb.ToString();
    }

    public virtual bool ParseArguments(string[] args)
    {
      return Parser.ParseArguments(args, this, Console.Out);
    }

    public virtual RunOptions GetRunOptions()
    {
      return new RunOptions(IncludeTags ?? new string[0], ExcludeTags ?? new string[0]);
    }

  }
}
