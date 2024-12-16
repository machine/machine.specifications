using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio
{
    public class MspecTestExecutor
    {
        private readonly ISpecificationExecutor executor;

        private readonly MspecTestDiscoverer discover;

        private readonly ISpecificationFilterProvider specificationFilterProvider;

        public MspecTestExecutor(ISpecificationExecutor executor, MspecTestDiscoverer discover, ISpecificationFilterProvider specificationFilterProvider)
        {
            this.executor = executor;
            this.discover = discover;
            this.specificationFilterProvider = specificationFilterProvider;
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Source Specifications.");

            var testsToRun = new List<TestCase>();

            DiscoverTests(sources, frameworkHandle, testsToRun);
            RunTests(testsToRun, runContext, frameworkHandle);

            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Source Specifications Complete.");
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Test Specifications.");

            var totalSpecCount = 0;
            var executedSpecCount = 0;
            var currentAssembly = string.Empty;

            try
            {
                var testCases = tests.ToArray();

                foreach (var grouping in testCases.GroupBy(x => x.Source))
                {
                    currentAssembly = grouping.Key;
                    totalSpecCount += grouping.Count();

                    var filteredTests = specificationFilterProvider.FilteredTests(grouping.AsEnumerable(), runContext, frameworkHandle);

                    var testsToRun = filteredTests
                        .Select(test => test.ToVisualStudioTestIdentifier())
                        .ToArray();

                    frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Executing {testsToRun.Length} tests in '{currentAssembly}'.");

                    executor.RunAssemblySpecifications(grouping.Key, testsToRun, MspecTestRunner.Uri, frameworkHandle);

                    executedSpecCount += testsToRun.Length;
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Execution Complete - {executedSpecCount} of {totalSpecCount} specifications in {testCases.GroupBy(x => x.Source).Count()} assemblies.");
            }
            catch (Exception exception)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, $"Machine Specifications Visual Studio Test Adapter - Error while executing specifications in assembly '{currentAssembly}'." + Environment.NewLine + exception);
            }
        }

        private void DiscoverTests(IEnumerable<string> sources, IMessageLogger logger, List<TestCase> testsToRun)
        {
            discover.DiscoverTests(sources, logger, testsToRun.Add);
        }
    }
}
