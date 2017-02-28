using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

using Machine.Specifications.ConsoleRunner.Outputs;
using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Reporting.Generation.Xml;
using Machine.Specifications.Reporting.Integration;
using Machine.Specifications.Reporting.Integration.TeamCity;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ConsoleRunner
{
    using Reporting.Integration.AppVeyor;

    public class Program
    {
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        [STAThread]
        public static void Main(string[] args)
        {
            var program = new Program(new DefaultConsole());
            ExitCode exitCode = program.Run(args);

            Environment.Exit((int)exitCode);
        }

        readonly IConsole _console;

        public Program(IConsole console)
        {
            _console = console;
        }

        public ExitCode Run(string[] arguments)
        {
            ExceptionReporter reporter = new ExceptionReporter(_console);

            Options options = new Options();
            if (!options.ParseArguments(arguments))
            {
                _console.WriteLine(Options.Usage());
                return ExitCode.Failure;
            }

            var timer = new TimingRunListener();
            var listeners = new List<ISpecificationRunListener>
                      {
                        timer
                      };

            string teamCityProjectName = Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME");
            string appVeyorApiUrl = Environment.GetEnvironmentVariable("APPVEYOR_API_URL");

            ISpecificationRunListener mainListener;
            if (options.TeamCityIntegration || (!options.DisableTeamCityAutodetection && teamCityProjectName != null))
            {
                mainListener = new TeamCityReporter(_console.WriteLine, timer);
            }
            else if (options.AppVeyorIntegration || (!options.DisableAppVeyorAutodetection && appVeyorApiUrl != null))
            {
                mainListener = new AppVeyorReporter(_console.WriteLine, new AppVeyorBuildWorkerApiClient(appVeyorApiUrl));
            }
            else
            {
                mainListener = new RunListener(_console, DetermineOutput(options, _console), timer);
            }

            try
            {

                if (!String.IsNullOrEmpty(options.HtmlPath))
                {
                    if (IsHtmlPathValid(options.HtmlPath))
                    {
                        listeners.Add(GetHtmlReportListener(options));
                    }
                    else
                    {
                        _console.WriteLine("Invalid html path: {0}", options.HtmlPath);
                        _console.WriteLine(Options.Usage());
                        return ExitCode.Failure;
                    }

                }

                if (!String.IsNullOrEmpty(options.XmlPath))
                {
                    if (IsHtmlPathValid(options.XmlPath))
                    {
                        listeners.Add(GetXmlReportListener(options, timer));
                    }
                    else
                    {
                        _console.WriteLine("Invalid xml path: {0}", options.XmlPath);
                        _console.WriteLine(Options.Usage());
                        return ExitCode.Failure;
                    }
                }

                listeners.Insert(0, mainListener);

                if (options.AssemblyFiles.Count == 0)
                {
                    _console.WriteLine(Options.Usage());
                    return ExitCode.Failure;
                }

                var listener = new AggregateRunListener(listeners);

                ISpecificationRunner appDomainRunner = new AppDomainRunner(listener, options.GetRunOptions());
                var assemblies = new List<AssemblyPath>();
                foreach (string assemblyName in options.AssemblyFiles)
                {
                    if (!File.Exists(assemblyName))
                    {
                        throw NewException.MissingAssembly(assemblyName);
                    }

                    var excludedAssemblies = new[] { "Machine.Specifications.dll", "Machine.Specifications.Clr4.dll" };
                    if (excludedAssemblies.Any(x => Path.GetFileName(assemblyName) == x))
                    {
                        _console.WriteLine("Warning: Excluded {0} from the test run because the file name matches either of these: {1}", assemblyName, string.Join(", ", excludedAssemblies));
                        continue;
                    }

                    assemblies.Add(new AssemblyPath(Assembly.ReflectionOnlyLoadFrom(assemblyName).Location));
                }

                if (options.WaitForDebugger)
                {
                    WaitForDebugger();
                    if (Debugger.IsAttached == false)
                    {
                        _console.WriteLine("Fatal error: Timeout while waiting for debugger to attach");
                        return ExitCode.Failure;
                    }
                }

                appDomainRunner.RunAssemblies(assemblies);
            }
            catch (Exception ex)
            {
                reporter.ReportException(ex);
                return ExitCode.Error;
            }

            if (mainListener is ISpecificationResultProvider)
            {
                var errorProvider = (ISpecificationResultProvider)mainListener;
                if (errorProvider.FailureOccurred)
                {
                    return ExitCode.Failure;
                }
            }
            return ExitCode.Success;
        }

        static IOutput DetermineOutput(Options options, IConsole console)
        {
            IOutput output = new VerboseOutput(console);

            if (options.Silent)
            {
                output = new SilentOutput();
            }
            if (options.Progress)
            {
                output = new ProgressOutput(console);
            }
            if (options.NoColor)
            {
                return output;
            }

            return new ColorOutput(output);
        }

        void WaitForDebugger()
        {
            var waitTime = TimeSpan.FromMilliseconds(200);
            var countdown = TimeSpan.FromMilliseconds(15000);

            _console.WriteLine("Waiting {0} seconds for debugger to attach", countdown.TotalSeconds);

            while (Debugger.IsAttached == false && countdown >= TimeSpan.Zero)
            {
                Thread.Sleep(waitTime);
                countdown = countdown.Subtract(waitTime);
            }
        }

        private static ISpecificationRunListener GetXmlReportListener(Options options, TimingRunListener timer)
        {
            var listener = new GenerateXmlReportListener(options.XmlPath, timer, options.ShowTimeInformation);

            return listener;
        }

        static bool IsHtmlPathValid(string path)
        {
            return Directory.Exists(Path.GetDirectoryName(path));
        }

        public ISpecificationRunListener GetHtmlReportListener(Options options)
        {
            return new GenerateSparkHtmlReportRunListener(options.HtmlPath, options.ShowTimeInformation);
        }
    }
}