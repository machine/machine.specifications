using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CommandLine;

using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class Options
  {
    [Option(null, "xml", HelpText = "Outputs the XML report to the file referenced by the path")]
    public string XmlPath = string.Empty;

    [Option(null,
      "html",
      HelpText = "Outputs the HTML report to path, one-per-assembly w/ index.html (if directory, otherwise all are in one file)")]
    public string HtmlPath = string.Empty;

    [Option("f",
      "filter",
      HelpText = "Filter file specifying contexts to execute (full type name, one per line). Takes precedence over tags")]
    public string FilterFile = string.Empty;

    [Option("s",
      "silent",
      HelpText = "Suppress progress output (print fatal errors, failures and summary)")]
    public bool Silent = false;

    [Option("p",
      "progress",
      HelpText = "Print dotted progress output")]
    public bool Progress = false;

    [Option("c",
      "no-color",
      HelpText = "Suppress colored console output")]
    public bool NoColor = false;

    [Option("t",
      "timeinfo",
      HelpText = "Adds time-related information in HTML output")]
    public bool ShowTimeInformation = false;

    [Option(null,
      "teamcity",
      HelpText = "Reporting for TeamCity CI integration (also auto-detected)")]
    public bool TeamCityIntegration = false;

    [Option(null,
      "no-teamcity-autodetect",
      HelpText = "Disables TeamCity autodetection")]
    public bool DisableTeamCityAutodetection = false;

    [OptionList("i",
      "include",
      HelpText = "Execute all specifications in contexts with these comma delimited tags. Ex. -i \"foo,bar,foo_bar\"",
      Separator = ',')]
    public IList<string> IncludeTags = null;

    [OptionList("x",
      "exclude",
      HelpText = "Exclude specifications in contexts with these comma delimited tags. Ex. -x \"foo,bar,foo_bar\"",
      Separator = ',')]
    public IList<string> ExcludeTags = null;

    [ValueList(typeof(List<string>))]
    public IList<string> AssemblyFiles = null;

    [Option("w",
      "wait",
      HelpText = "Wait for debugger to be attached")]
    public bool WaitForDebugger;

    [HelpOption]
    public string GetUsage()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Machine.Specifications");
      sb.AppendLine("Copyright (C) 2007-2013");
      sb.AppendLine("");
      sb.AppendLine(Usage());
      sb.AppendLine("Options:");
	  sb.AppendLine("  -f, --filters               Filter file specifying contexts to execute (full type name, one per line). Takes precedence over tags");
      sb.AppendLine("  -i, --include               Execute all specifications in contexts with these comma delimited tags. Ex. -i \"foo,bar,foo_bar\"");
      sb.AppendLine("  -x, --exclude               Exclude specifications in contexts with these comma delimited tags. Ex. -x \"foo,bar,foo_bar\"");
      sb.AppendLine("  -t, --timeinfo              Shows time-related information in HTML output");
      sb.AppendLine("  -s, --silent                Suppress progress output (print fatal errors, failures and summary)");
      sb.AppendLine("  -p, --progress              Print progress output");
      sb.AppendLine("  -c, --no-color              Suppress colored console output");
      sb.AppendLine("  -w, --wait                  Wait 15 seconds for debugger to be attached");
      sb.AppendLine("  --teamcity                  Reporting for TeamCity CI integration (also auto-detected)");
      sb.AppendLine("  --no-teamcity-autodetect    Disables TeamCity autodetection");
      sb.AppendLine("  --html <PATH>               Outputs the HTML report to path, one-per-assembly w/ index.html (if directory, otherwise all are in one file)");
      sb.AppendLine("  --xml <PATH>                Outputs the XML report to the file referenced by the path");
      sb.AppendLine("  -h, --help                  Shows this help message");

      return sb.ToString();
    }

    public static string Usage()
    {
      var runnerExe = Assembly.GetEntryAssembly();
      return String.Format(Resources.UsageStatement, Path.GetFileName(runnerExe != null ? runnerExe.Location : "mspec.exe"));
    }

    public virtual bool ParseArguments(string[] args)
    {
      return new CommandLineParser().ParseArguments(args, this, Console.Out);
    }

    public virtual RunOptions GetRunOptions()
    {
      var filters = new string[0];
      if (!String.IsNullOrEmpty(FilterFile))
      {
        filters = File.ReadAllLines(FilterFile, Encoding.UTF8);
      }

      return new RunOptions(IncludeTags ?? new string[0], ExcludeTags ?? new string[0], filters);
    }
  }
}
