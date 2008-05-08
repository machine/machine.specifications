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
using Gallio.Reflection;
using Machine.Specifications.Example;
using Machine.Specifications.GallioAdapter.Model;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
  [TestFixture]
  public class FrameworkTests : BaseTestFrameworkTest
  {
    protected override Gallio.Model.ITestFramework CreateFramework()
    {
      return new MachineSpecificationsFramework();
    }

    protected override System.Reflection.Assembly GetSampleAssembly()
    {
      return typeof(Account).Assembly;
    }

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
      Version expectedVersion = typeof(DescriptionAttribute).Assembly.GetName().Version;
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
    public void DescriptionTestShouldBeValid()
    {
      PopulateTestTree();
      RootTest rootTest = testModel.RootTest;
      BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
      BaseTest assemblyTest = (BaseTest)frameworkTest.Children[0];
      MachineDescriptionTest fixtureTest =
        (MachineDescriptionTest)
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
      MachineDescriptionTest fixtureTest =
        (MachineDescriptionTest)
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
  }
}
