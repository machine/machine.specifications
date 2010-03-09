// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
// 
// Modified by and Portions Copyright 2008 Machine Project

using System;
using System.IO;
using System.Reflection;
using Gallio.Common.Messaging;
using Gallio.Model.Contexts;
using Gallio.Model.Isolation;
using Gallio.Model.Messages;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Loader;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Runtime.Logging;
using NUnit.Framework;
using Gallio.Model.Environments;
using Gallio.Runtime.ProgressMonitoring;
using Rhino.Mocks;
using Test = Gallio.Model.Tree.Test;

namespace Machine.Specifications.GallioAdapter.Tests
{
    public abstract class BaseTestFrameworkTest<TSampleFixture>
    {        
        protected Assembly SimpleFixtureAssembly
        {
            get { return SimpleFixtureType.Assembly; }
        }

        protected Type SimpleFixtureType
        {
            get { return typeof(TSampleFixture); }
        }

        protected string SimpleFixtureNamespace
        {
            get { return SimpleFixtureType.Namespace; }
        }

        protected abstract ComponentHandle<ITestFramework, TestFrameworkTraits> TestFrameworkHandle { get; }

        protected virtual string AssemblyKind
        {
            get { return TestKinds.Assembly; }
        }

        protected virtual string PassTestName
        {
            get { return "Pass"; }
        }

        protected virtual string FailTestName
        {
            get { return "Fail"; }
        }

        protected TestModel PopulateTestTree()
        {
            return PopulateTestTree(SimpleFixtureAssembly);
        }

        protected TestModel PopulateTestTree(Assembly assembly)
        {
            TestModel testModel = new TestModel();

            var testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            var logger = new MarkupStreamLogger(TestLog.Default);

            var testFrameworkSelector = new TestFrameworkSelector()
            {
                Filter = testFrameworkHandle => testFrameworkHandle.Id == TestFrameworkHandle.Id,
                FallbackMode = TestFrameworkFallbackMode.Strict
            };

            ITestDriver testDriver = testFrameworkManager.GetTestDriver(testFrameworkSelector, logger);

            var testIsolationProvider = (ITestIsolationProvider)RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.LocalTestIsolationProvider");
            var testIsolationOptions = new TestIsolationOptions();
            using (ITestIsolationContext testIsolationContext = testIsolationProvider.CreateContext(testIsolationOptions, logger))
            {
                var testPackage = new TestPackage();
                testPackage.AddFile(new FileInfo(AssemblyUtils.GetFriendlyAssemblyCodeBase(assembly)));
                var testExplorationOptions = new TestExplorationOptions();

                var messageSink = TestModelSerializer.CreateMessageSinkToPopulateTestModel(testModel);

                new LogProgressMonitorProvider(logger).Run(progressMonitor =>
                {
                    testDriver.Explore(testIsolationContext, testPackage, testExplorationOptions,
                        messageSink, progressMonitor);
                });
            }

            return testModel;
        }

        protected Test GetDescendantByName(Test parent, string name)
        {
            foreach (Test test in parent.Children)
            {
                if (test.Name == name)
                    return test;

                Test descendant = GetDescendantByName(test, name);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }

        protected TestParameter GetParameterByName(Test test, string name)
        {
            foreach (TestParameter testParameter in test.Parameters)
            {
                if (testParameter.Name == name)
                    return testParameter;
            }

            return null;
        }        
    }
}