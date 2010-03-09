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
using System.Text;
using Gallio.Model;
using Gallio.Common.Reflection;
using Machine.Specifications.Example;
using Machine.Specifications.GallioAdapter.Model;
using Gallio.Framework.Utilities;
using NUnit.Framework;
using Machine.Specifications.GallioAdapter.TestResources;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime;
using Gallio.Model.Tree;

namespace Machine.Specifications.GallioAdapter.Tests
{
    // Adapted from the Gallio BaseTestFrameworkTest<SimpleTest> Test
    [TestFixture]
    public class FrameworkTests : BaseTestFrameworkTest<simple_test_spec>
    {
        const string ParentTestName = "simple test spec";
        const string PassTestName = "pass";
        const string FailTestName = "fail";

        private void AssertStringContains(string needle, string haystack)
        {
            // HACK: needed a quick string comparison
            StringAssert.Contains(needle.ToUpper(), haystack.ToUpper());
        }

        protected override ComponentHandle<ITestFramework, TestFrameworkTraits> TestFrameworkHandle
        {
            get
            {
                return (ComponentHandle<ITestFramework, TestFrameworkTraits>)
                    RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("Machine.Specifications");
            }
        }
        protected override string AssemblyKind
        {
            get
            {
                return TestKinds.Assembly;
            }
        }        

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
            Assert.AreEqual("Machine.Specifications.Adapter.TestResources Product", assemblyTest.Metadata.GetValue(MetadataKeys.Product));
            Assert.AreEqual("Machine.Specifications.Adapter.TestResources", assemblyTest.Metadata.GetValue(MetadataKeys.Title));

            Assert.AreEqual("1.2.3.4", assemblyTest.Metadata.GetValue(MetadataKeys.InformationalVersion));
            Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.FileVersion));
            Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.Version));
        }
#if FALSE
        [Test]
    public void RootTestShouldBeValid()
    {
      PopulateTestTree();

      RootTest rootTest = testModel.RootTest;
      Assert.IsNull(rootTest.Parent);
      Assert.AreEqual(TestKinds.Root, rootTest.Kind);
      Assert.IsNull(rootTest.CodeElement);
      Assert.IsFalse(rootTest.IsTestCase);
      Assert.AreEqual(1, rootTest.Children.Count);
    }

    [Test]
    public void BaseTestShouldBeValid()
    {
      PopulateTestTree();
      RootTest rootTest = testModel.RootTest;
      Version expectedVersion = typeof(SubjectAttribute).Assembly.GetName().Version;
      BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
      Assert.AreSame(testModel.RootTest, frameworkTest.Parent);
      Assert.AreEqual(TestKinds.Framework, frameworkTest.Kind);
      Assert.IsNull(frameworkTest.CodeElement);
      Assert.AreEqual("Machine.Specifications v" + expectedVersion, frameworkTest.Name);
      Assert.IsFalse(frameworkTest.IsTestCase);
      Assert.AreEqual(1, frameworkTest.Children.Count);
    }

    [Test]
    public void AssemblyTestShouldBeValid()
    {
      PopulateTestTree();
      RootTest rootTest = testModel.RootTest;
      BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
      BaseTest assemblyTest = (BaseTest)frameworkTest.Children[0];
      Assert.AreSame(frameworkTest, assemblyTest.Parent);
      Assert.AreEqual(TestKinds.Assembly, assemblyTest.Kind);
      Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTest.CodeElement.CodeReference);
      Assert.AreEqual(sampleAssembly.GetName().Name, assemblyTest.Name);
      Assert.IsFalse(assemblyTest.IsTestCase);
      Assert.GreaterOrEqual(assemblyTest.Children.Count, 1);
    }

    [Test]
    public void ContextTestShouldBeValid()
    {
      PopulateTestTree();
      RootTest rootTest = testModel.RootTest;
      BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
      BaseTest assemblyTest = (BaseTest)frameworkTest.Children[0];
      MachineContextTest fixtureTest =
        (MachineContextTest)
          GetDescendantByName(assemblyTest, "Transferring between from account and to account");
      Assert.AreSame(assemblyTest, fixtureTest.Parent);
      Assert.AreEqual(TestKinds.Fixture, fixtureTest.Kind);
      Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "Machine.Specifications.Example", "Machine.Specifications.Example.Transferring_between_from_account_and_to_account", null, null),
          fixtureTest.CodeElement.CodeReference);
      Assert.AreEqual("Transferring between from account and to account", fixtureTest.Name);
      Assert.IsFalse(fixtureTest.IsTestCase);
      Assert.AreEqual(2, fixtureTest.Children.Count);
    }

    [Test]
    public void SpecificationTestShouldBeValid()
    {
      PopulateTestTree();
      RootTest rootTest = testModel.RootTest;
      BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
      BaseTest assemblyTest = (BaseTest)frameworkTest.Children[0];
      MachineContextTest fixtureTest =
        (MachineContextTest)
          GetDescendantByName(assemblyTest, "Transferring between from account and to account");

      MachineSpecificationTest test = (MachineSpecificationTest) GetDescendantByName(fixtureTest, "should debit the from account by the amount transferred");
      Assert.AreSame(fixtureTest, test.Parent);
      Assert.AreEqual(TestKinds.Test, test.Kind);
      Assert.AreEqual(
        new CodeReference(sampleAssembly.FullName, "Machine.Specifications.Example",
          "Machine.Specifications.Example.Transferring_between_from_account_and_to_account", "should_debit_the_from_account_by_the_amount_transferred", null),
        test.CodeElement.CodeReference);
      Assert.AreEqual("should debit the from account by the amount transferred", test.Name);
      Assert.IsTrue(test.IsTestCase);
      Assert.AreEqual(0, test.Children.Count);
    }
#endif
    }
}
