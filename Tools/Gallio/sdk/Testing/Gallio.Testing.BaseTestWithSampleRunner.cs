// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Framework.Utilities;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;

namespace Gallio.Testing
{
    /// <summary>
    /// Abstract test fixture that provides a sample test runner.
    /// </summary>
    [Disable]
    public abstract class BaseTestWithSampleRunner
    {
        private readonly SampleRunner runner = new SampleRunner();
        private static bool isSampleRunning;

        /// <summary>
        /// Gets the sample test runner.
        /// </summary>
        protected SampleRunner Runner
        {
            get
            {
                return runner;
            }
        }

        /// <summary>
        /// Gets the test report.
        /// </summary>
        protected Report Report
        {
            get
            {
                return Runner.Report;
            }
        }

        [FixtureSetUp]
        public void RunDeclaredSamples()
        {
            if (isSampleRunning)
                return;

            try
            {
                isSampleRunning = true;
                ConfigureRunner();

                if (runner.TestPackage.Files.Count != 0)
                    runner.Run();
            }
            finally
            {
                isSampleRunning = false;
            }
        }

        /// <summary>
        /// Configures the sample test runner according to the declared [RunSample] attributes decorating the test fixture.
        /// </summary>
        protected virtual void ConfigureRunner()
        {
            foreach (RunSampleAttribute attrib in GetType().GetCustomAttributes(typeof(RunSampleAttribute), true))
            {
                if (attrib.MethodName.Length == 0)
                {
                    runner.AddFixture(attrib.FixtureType);
                }
                else
                {
                    runner.AddMethod(attrib.FixtureType, attrib.MethodName);
                }
            }
        }

        /// <summary>
        /// Returns the log of the specified test step run.
        /// </summary>
        /// <param name="run">The test step run.</param>
        /// <returns>The log text or an empty string.</returns>
        protected static string GetLog(TestStepRun run)
        {
            return GetLog(run, MarkupStreamNames.Default);
        }

        /// <summary>
        /// Returns the log of the specified test step run.
        /// </summary>
        /// <param name="run">The test step run.</param>
        /// <param name="streamName">The name of log stream.</param>
        /// <returns>The log text or an empty string.</returns>
        protected static string GetLog(TestStepRun run, string streamName)
        {
            StructuredStream stream = run.TestLog.GetStream(streamName);
            return (stream == null) ? String.Empty : stream.ToString();
        }

        /// <summary>
        /// Returns the default non-empty logs of the specified test step runs.
        /// </summary>
        /// <param name="runs">The test step runs.</param>
        /// <returns>An enumeration of non-empty logs.</returns>
        protected static IEnumerable<string> GetLogs(IEnumerable<TestStepRun> runs)
        {
            foreach (TestStepRun run in runs)
            {
                string log = GetLog(run);

                if (!String.IsNullOrEmpty(log))
                    yield return log;
            }
        }

        /// <summary>
        /// Returns all the test step runs for the specified test method.
        /// </summary>
        /// <param name="fixtureType">The searched fixture type.</param>
        /// <param name="testMethodName">The name of the test method.</param>
        /// <returns>An enumeration of the test step runs.</returns>
        protected IEnumerable<TestStepRun> GetTestStepRuns(Type fixtureType, string testMethodName)
        {
            return Runner.GetTestStepRuns(CodeReference.CreateFromMember(fixtureType.GetMethod(testMethodName)));
        }

        /// <summary>
        /// Returns the primary test step run for the specified test method.
        /// </summary>
        /// <param name="fixtureType">The searched fixture type.</param>
        /// <param name="testMethodName">The name of the test method.</param>
        /// <returns>The test step run, or a null reference if not found.</returns>
        protected TestStepRun GetPrimaryTestStepRun(Type fixtureType, string testMethodName)
        {
            return Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(fixtureType.GetMethod(testMethodName)));
        }
    }
}