using System;
using System.Collections.Generic;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio
{
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [ExtensionUri(ExecutorUri)]
    [DefaultExecutorUri(ExecutorUri)]
    public class MspecTestRunner : ITestDiscoverer, ITestExecutor
    {
        private const string ExecutorUri = "executor://machine.vstestadapter";

        public static readonly Uri Uri = new Uri(ExecutorUri);

        private readonly MspecTestDiscoverer testDiscoverer;

        private readonly MspecTestExecutor testExecutor;

        public MspecTestRunner()
            : this(new BuiltInSpecificationDiscoverer(), new SpecificationExecutor(), new SpecificationFilterProvider())
        {
        }

        public MspecTestRunner(ISpecificationDiscoverer discoverer, ISpecificationExecutor executor, ISpecificationFilterProvider specificationFilterProvider)
        {
            testDiscoverer = new MspecTestDiscoverer(discoverer);
            testExecutor = new MspecTestExecutor(executor, testDiscoverer, specificationFilterProvider);
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            testDiscoverer.DiscoverTests(sources, logger, discoverySink);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            testExecutor.RunTests(tests, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            testExecutor.RunTests(sources, runContext, frameworkHandle);
        }

        public void Cancel()
        {
        }
    }
}
