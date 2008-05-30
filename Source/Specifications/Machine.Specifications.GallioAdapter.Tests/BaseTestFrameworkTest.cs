// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Runner.Harness;
using Gallio.Runtime;
using Gallio.Runtime.Loader;
using Gallio.Runtime.ProgressMonitoring;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
  public abstract class BaseTestFrameworkTest
  {
    protected Assembly sampleAssembly;
    protected ITestFramework framework;
    protected TestModel testModel;

    ITestHarness harness;

    protected abstract Assembly GetSampleAssembly();

    protected abstract ITestFramework CreateFramework();

    [SetUp]
    public void SetUp()
    {
      sampleAssembly = GetSampleAssembly();

      harness = new DefaultTestHarness(TestContextTrackerAccessor.GetInstance(),
        RuntimeAccessor.Instance.Resolve<ILoader>());

      framework = CreateFramework();
      harness.AddTestFramework(framework);
    }

    [TearDown]
    public void TearDown()
    {
      if (harness != null)
      {
        harness.Dispose();
        harness = null;
        framework = null;
        sampleAssembly = null;
      }
    }

    protected void PopulateTestTree()
    {
      TestPackageConfig config = new TestPackageConfig();
      config.AssemblyFiles.Add(AssemblyUtils.GetFriendlyAssemblyCodeBase(sampleAssembly));

      harness.Load(config, NullProgressMonitor.CreateInstance());
      harness.Explore(new TestExplorationOptions(), NullProgressMonitor.CreateInstance());

      testModel = harness.TestModel;
    }

    protected ITest GetDescendantByName(ITest parent, string name)
    {
      foreach (ITest test in parent.Children)
      {
        if (test.Name == name)
          return test;

        ITest descendant = GetDescendantByName(test, name);
        if (descendant != null)
          return descendant;
      }

      return null;
    }

    protected ITestParameter GetParameterByName(ITest test, string name)
    {
      foreach (ITestParameter testParameter in test.Parameters)
      {
        if (testParameter.Name == name)
          return testParameter;
      }

      return null;
    }
  }
}