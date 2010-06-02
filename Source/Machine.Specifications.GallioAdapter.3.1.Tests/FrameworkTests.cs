// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
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
using System.IO;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Isolation;
using Gallio.Model.Messages;
using Gallio.Model.Tree;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.GallioAdapter.TestResources;
using NUnit.Framework;
using Test = Gallio.Model.Tree.Test;

namespace Machine.Specifications.GallioAdapter.Tests
{
  // Adapted from the Gallio BaseTestFrameworkTest<TSimpleTest> Test
  [TestFixture]
  public class FrameworkTests
  {    
    const string ParentTestName = "simple test spec";
    const string PassTestName = "pass";
    const string FailTestName = "fail";
    readonly string AssemblyKind = TestKinds.Assembly;
    readonly Assembly SimpleFixtureAssembly = typeof(simple_test_spec).Assembly;
    readonly Type SimpleFixtureType = typeof(simple_test_spec);
    readonly string SimpleFixtureNamespace = typeof(simple_test_spec).Namespace;

    void AssertStringContains(string needle, string haystack)
    {
      // HACK: needed a quick string comparison that was not case sensitive
      StringAssert.Contains(needle.ToUpper(), haystack.ToUpper());
    }

    #region Members from the original Gallio class BaseTestFrameworkTest<TSimpleTest>
    ComponentHandle<ITestFramework, TestFrameworkTraits> TestFrameworkHandle
    {
      get
      {
        return (ComponentHandle<ITestFramework, TestFrameworkTraits>)
          RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("Machine.Specifications");
      }
    }

    TestModel PopulateTestTree()
    {
      return PopulateTestTree(SimpleFixtureAssembly);
    }

    TestModel PopulateTestTree(Assembly assembly)
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

    Test GetDescendantByName(Test parent, string name)
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
    #endregion    

    [Test]
    public void PopulateTreeTest_IgnoredContextShouldIncludeExtraMetadata()
    {
      TestModel testModel = PopulateTestTree();

      Test test = GetDescendantByName(testModel.RootTest.Children[0], "ignored context spec");
      
      Assert.IsNotNull(test);
      AssertStringContains("Attribute", test.Metadata.GetValue(MetadataKeys.IgnoreReason));      
    }

    [Test]
    public void PopulateTreeTest_IgnoredSpecificationShouldIncludeExtraMetadata()
    {
      TestModel testModel = PopulateTestTree();

      Test context = GetDescendantByName(testModel.RootTest.Children[0], "ignored specification spec");
      Assert.IsNotNull(context);

      Test spec = GetDescendantByName(context, "should");
      Assert.IsNotNull(spec);

      StringAssert.DoesNotContain("Attribute", context.Metadata.GetValue(MetadataKeys.IgnoreReason));
      AssertStringContains("Attribute", spec.Metadata.GetValue(MetadataKeys.IgnoreReason));
    }

    [Test]
    public void PopulateTreeTest_IgnoredSpecificationDueToIgnoredContextShouldIndicateIgnoredDueToParent()
    {
      TestModel testModel = PopulateTestTree();

      Test context = GetDescendantByName(testModel.RootTest.Children[0], "ignored context spec");
      Assert.IsNotNull(context);

      Test spec = GetDescendantByName(context, "should");
      Assert.IsNotNull(spec);

      AssertStringContains("Attribute", spec.Metadata.GetValue(MetadataKeys.IgnoreReason));
      AssertStringContains("Context", spec.Metadata.GetValue(MetadataKeys.IgnoreReason));
    }

    [Test]
    public void PopulateTreeTest_SubjectShouldBeSavedAsTheCategory()
    {
      TestModel testModel = PopulateTestTree();

      Test test = GetDescendantByName(testModel.RootTest.Children[0], "subject spec");

      Assert.IsNotNull(test);    
      
      string category = test.Metadata.GetValue(MetadataKeys.Category);

      AssertStringContains("Testing out the framework", category);  // Make sure the text is there
      AssertStringContains("bool", category);             // Make sure the type is there too
    }

    [Test]
    public void PopulateTreeTest_TagShouldAddExtraMetaData()
    {
      TestModel testModel = PopulateTestTree();

      Test test = GetDescendantByName(testModel.RootTest.Children[0], "tag spec");

      Assert.IsNotNull(test);

      IList<string> tags = test.Metadata[SpecificationMetadataKeys.Tags];

      tags.Contains("tag").ShouldBeTrue();
    }

    [Test]
    public void PopulateTreeTest_MultipleTagsShouldContainIndividualEntries()
    {
      TestModel testModel = PopulateTestTree();

      Test test = GetDescendantByName(testModel.RootTest.Children[0], "multiple tag spec");

      Assert.IsNotNull(test);

      IList<string> tags = test.Metadata[SpecificationMetadataKeys.Tags];

      tags.Contains("one").ShouldBeTrue();
      tags.Contains("two").ShouldBeTrue();
      tags.Contains("three").ShouldBeTrue();
    }

    // These tests are borrowed directly from the Gallio MS Test adapter tests
    
    [Test]
    public void PopulateTestTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
    {
      TestModel testModel = PopulateTestTree(typeof(int).Assembly);

      Assert.AreEqual(0, testModel.RootTest.Children.Count);
    }

    [Test]
    public void PopulateTestTree_CapturesTestStructureAndBasicMetadata()
    {
      TestModel testModel = PopulateTestTree();

      Test rootTest = testModel.RootTest;
      Assert.IsNull(rootTest.Parent);
      Assert.AreEqual(TestKinds.Root, rootTest.Kind);
      Assert.IsNull(rootTest.CodeElement);
      Assert.IsFalse(rootTest.IsTestCase);
      Assert.AreEqual(1, rootTest.Children.Count);

      Test assemblyTest = rootTest.Children[0];
      Assert.AreSame(rootTest, assemblyTest.Parent);
      Assert.AreEqual(AssemblyKind, assemblyTest.Kind);
      AssertStringContains(SimpleFixtureAssembly.Location, assemblyTest.Metadata.GetValue(MetadataKeys.File));
      Assert.AreEqual(CodeReference.CreateFromAssembly(SimpleFixtureAssembly), assemblyTest.CodeElement.CodeReference);
      Assert.AreEqual(SimpleFixtureAssembly.GetName().Name, assemblyTest.Name);
      Assert.IsFalse(assemblyTest.IsTestCase);
      Assert.GreaterOrEqual(assemblyTest.Children.Count, 1);

      Test fixtureTest = GetDescendantByName(assemblyTest, ParentTestName);
      Assert.AreEqual(TestKinds.Fixture, fixtureTest.Kind);
      Assert.AreEqual(new CodeReference(SimpleFixtureAssembly.FullName, SimpleFixtureNamespace, SimpleFixtureNamespace + ".simple_test_spec", null, null),
        fixtureTest.CodeElement.CodeReference);
      Assert.AreEqual(ParentTestName, fixtureTest.Name);
      Assert.IsFalse(fixtureTest.IsTestCase);
      Assert.AreEqual(2, fixtureTest.Children.Count);

      Test passTest = GetDescendantByName(fixtureTest, PassTestName);
      Test failTest = GetDescendantByName(fixtureTest, FailTestName);

      Assert.IsNotNull(passTest, "Cannot find test case '{0}'", PassTestName);
      Assert.IsNotNull(failTest, "Cannot find test case '{0}'", FailTestName);

      Assert.AreSame(fixtureTest, passTest.Parent);
      Assert.AreEqual(TestKinds.Test, passTest.Kind);
      Assert.AreEqual(new CodeReference(SimpleFixtureAssembly.FullName, SimpleFixtureNamespace, SimpleFixtureNamespace + ".simple_test_spec", PassTestName, null),
        passTest.CodeElement.CodeReference);
      Assert.AreEqual(PassTestName, passTest.Name);
      Assert.IsTrue(passTest.IsTestCase);
      Assert.AreEqual(0, passTest.Children.Count);

      Assert.AreSame(fixtureTest, failTest.Parent);
      Assert.AreEqual(TestKinds.Test, failTest.Kind);
      Assert.AreEqual(new CodeReference(SimpleFixtureAssembly.FullName, SimpleFixtureNamespace, SimpleFixtureNamespace + ".simple_test_spec", FailTestName, null),
        failTest.CodeElement.CodeReference);
      Assert.AreEqual(FailTestName, failTest.Name);
      Assert.IsTrue(failTest.IsTestCase);
      Assert.AreEqual(0, failTest.Children.Count);
    }
    
    [Test]
    public void MetadataImport_XmlDocumentation()
    {
      TestModel testModel = PopulateTestTree();

      Test test = GetDescendantByName(testModel.RootTest, ParentTestName);
      Test passTest = GetDescendantByName(test, PassTestName);
      Test failTest = GetDescendantByName(test, FailTestName);

      Assert.AreEqual("<summary>\nA simple test specification.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
      Assert.AreEqual("<summary>\nA passing specification.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
      Assert.AreEqual("<summary>\nA failing specification.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
    }    

    [Test]
    public void MetadataImport_AssemblyAttributes()
    {      
      TestModel testModel = PopulateTestTree();

      Test assemblyTest = testModel.RootTest.Children[0];

      Assert.AreEqual("Machine Project", assemblyTest.Metadata.GetValue(MetadataKeys.Company));
      Assert.AreEqual("Test", assemblyTest.Metadata.GetValue(MetadataKeys.Configuration));
      AssertStringContains("Copyright © Machine Project 2008, 2009, 2010", assemblyTest.Metadata.GetValue(MetadataKeys.Copyright));
      Assert.AreEqual("Machine.Specifications.Adapter.TestResources Description", assemblyTest.Metadata.GetValue(MetadataKeys.Description));
      Assert.AreEqual("Machine.Specifications", assemblyTest.Metadata.GetValue(MetadataKeys.Product));
      Assert.AreEqual("Machine.Specifications.Adapter.TestResources", assemblyTest.Metadata.GetValue(MetadataKeys.Title));

      Assert.AreEqual("1.2.3.4", assemblyTest.Metadata.GetValue(MetadataKeys.InformationalVersion));
      Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.FileVersion));
      Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.Version));
    }
  }
}
