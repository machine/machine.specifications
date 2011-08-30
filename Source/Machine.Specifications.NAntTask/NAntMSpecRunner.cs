using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Reporting.Generation.Xml;
using Machine.Specifications.Reporting.Integration;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace Machine.Specifications.NAntTask
{
    [TaskName("mspec")]
    public class NAntMSpecRunnerTask : Task
    {
        readonly IConsole _console;

        public NAntMSpecRunnerTask()
        {
            AssemblyFiles = new FileSet();
            Tags = new PatternSet();
            _console = new NAntConsole(this);
            FailOnError = true;
        }

        [TaskAttribute("htmlPath", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string HtmlPath { get; set; }

        [TaskAttribute("xmlPath", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string XmlPath { get; set; }

        [TaskAttribute("showTime", Required = false)]
        public bool ShowTimeInformation { get; set; }

        [TaskAttribute("silent", Required = false)]
        public bool Silent { get; set; }

        [TaskAttribute("workingDirectory", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string WorkingDirectory { get; set; }

        [BuildElement("fileset", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public FileSet AssemblyFiles { get; set; }

        // TODO: Make a specialized TagSet
        [BuildElement("tags", Required = false)]
        public PatternSet Tags { get; set; }

        // TODO: Create a ContextSet

        protected override void ExecuteTask()
        {
            var listeners = new List<ISpecificationRunListener>();

            var timingListener = new TimingRunListener();
            listeners.Add(timingListener);

            string currentDirectory = Directory.GetCurrentDirectory();

            if (WorkingDirectory != null)
                Directory.SetCurrentDirectory(WorkingDirectory);

            // Detect if we are running in TeamCity
            ISpecificationRunListener mainListener = Project.Properties.Contains("teamcity.version")
                                                         ? (ISpecificationRunListener)new TeamCityReporter(x => Log(Level.Verbose, x), timingListener)
                                                         : new RunListener(_console, Silent, timingListener);

            try
            {

                if (!String.IsNullOrEmpty(HtmlPath))
                {
                    if (IsHtmlPathValid(HtmlPath))
                        listeners.Add(GetHtmlReportListener());
                    else
                        throw new BuildException("Invalid htmlPath:" + HtmlPath, Location);
                }

                if (!String.IsNullOrEmpty(XmlPath))
                {
                    if (IsHtmlPathValid(XmlPath))
                        listeners.Add(GetXmlReportListener(timingListener));
                    else
                        throw new BuildException("Invalid xml path:" + XmlPath, Location);
                }

                listeners.Add(mainListener);

                if (AssemblyFiles.FileNames.Count == 0)
                    return;

                var listener = new AggregateRunListener(listeners);

                ISpecificationRunner specificationRunner = new AppDomainRunner(listener, GetRunOptions());
                var assemblies = new List<Assembly>();
                foreach (string assemblyName in AssemblyFiles.FileNames)
                {
                    if (!File.Exists(assemblyName))
                        throw new BuildException("Could not find assembly: " + assemblyName);

                    Assembly assembly = Assembly.LoadFrom(assemblyName);
                    assemblies.Add(assembly);
                }

                specificationRunner.RunAssemblies(assemblies);
            }
            catch (Exception ex)
            {
                throw new BuildException("Error running mspec task", Location, ex);
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDirectory);
            }

            if (mainListener is ISpecificationResultProvider)
            {
                var errorProvider = (ISpecificationResultProvider) mainListener;
                if (errorProvider.FailureOccurred)
                    throw new BuildException("Tests failed", Location);
            }
        }

        RunOptions GetRunOptions()
        {
            return new RunOptions(Tags.GetIncludePatterns(), Tags.GetExcludePatterns(), new string[] {});
        }

        private ISpecificationRunListener GetXmlReportListener( TimingRunListener timer)
        {
            var listener = new GenerateXmlReportListener(XmlPath, timer, ShowTimeInformation);

            return listener;
        }

        static bool IsHtmlPathValid(string path)
        {
            return Directory.Exists(Path.GetDirectoryName(path) ?? String.Empty);
        }

        public ISpecificationRunListener GetHtmlReportListener()
        {
            return new GenerateSparkHtmlReportRunListener(HtmlPath, ShowTimeInformation);
        }
    }
}