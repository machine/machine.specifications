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
using System.Diagnostics;
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;
using Machine.Specifications.Explorers;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.GallioAdapter.Properties;
using Machine.Specifications.Model;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;

namespace Machine.Specifications.GallioAdapter.Services
{
    public class MachineSpecificationsExplorer : TestExplorer
    {
        private const string MachineSpecificationsAssemblyDisplayName = @"Machine.Specifications";
        
        public readonly Dictionary<IAssemblyInfo, Test> assemblyTests;
        public readonly Dictionary<ITypeInfo, Test> typeTests;

        public MachineSpecificationsExplorer()
        {
            assemblyTests = new Dictionary<IAssemblyInfo, Test>();
            typeTests = new Dictionary<ITypeInfo, Test>();
        }        

        protected override void ExploreImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
            if (assembly != null)
            {
                Version frameworkVersion = GetFrameworkVersion(assembly);
                if (frameworkVersion != null)
                {
                    ITypeInfo type = ReflectionUtils.GetType(codeElement);

                    Test assemblyTest = GetAssemblyTest(assembly, TestModel.RootTest, frameworkVersion, type == null);

                    //if (type != null)
                    //{
                    //    TryGetTypeTest(type, assemblyTest);
                    //}
                }
            }
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MachineSpecificationsAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private Test GetAssemblyTest(IAssemblyInfo assembly, Test parentTest, Version frameworkVersion, bool populateRecursively)
        {
            Test assemblyTest;
            if (!assemblyTests.TryGetValue(assembly, out assemblyTest))
            {
                assemblyTest = CreateAssemblyTest(assembly, frameworkVersion);

                string frameworkName = String.Format("Machine Specifications v{0}", frameworkVersion);
                assemblyTest.Metadata.SetValue(MetadataKeys.Framework, frameworkName);
                assemblyTest.Metadata.SetValue(MetadataKeys.File, assembly.Path);
                assemblyTest.Kind = TestKinds.Assembly;                

                parentTest.AddChild(assemblyTest);
                assemblyTests.Add(assembly, assemblyTest);                
            }

            if (populateRecursively)
            {
                AssemblyExplorer explorer = new AssemblyExplorer();
                var contexts = explorer.FindContextsIn(assembly.Resolve(false));                

                foreach (var context in contexts)
                {
                    MachineContextTest contextTest = new MachineContextTest(context);                    

                    foreach (var specification in context.Specifications)
                    {
                        MachineSpecificationTest specificationTest = new MachineSpecificationTest(specification);
                        AddXmlComment(specificationTest, Reflector.Wrap(specification.FieldInfo));
                        contextTest.AddChild(specificationTest);                                               
                    }

                    AddXmlComment(contextTest, Reflector.Wrap(context.Type));
                    assemblyTest.AddChild(contextTest);                                        
                }                
            }

            return assemblyTest;
        }

        private static Test CreateAssemblyTest(IAssemblyInfo assembly, Version frameworkVersion)
        {
            MachineAssembly assemblyTest = new MachineAssembly(assembly.Name, assembly, frameworkVersion);
            assemblyTest.Kind = TestKinds.Assembly;

            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

            return assemblyTest;
        }

        private Test TryGetTypeTest(ITypeInfo type, Test assemblyTest)
        {
            Test typeTest;
            if (!typeTests.TryGetValue(type, out typeTest))
            {
                try
                {                    
                    typeTest = CreateTypeTest(type);                    
                }
                catch (Exception ex)
                {
                    TestModel.AddAnnotation(new Annotation(AnnotationType.Error, type, "An exception was thrown while exploring an MSTest test type.", ex));
                }

                if (typeTest != null)
                {
                    assemblyTest.AddChild(typeTest);
                    typeTests.Add(type, typeTest);
                }
            }

            return typeTest;
        }

        private MachineGallioTest CreateTypeTest(ITypeInfo typeInfo)
        {                        
            return new MachineSpecificationTest( new Specification("Hello", () => Console.WriteLine("It"), true, null));
        }

        private void AddXmlComment(Test test, ICodeElementInfo element)
        {
            string xml = element.GetXmlDocumentation();
            if (!string.IsNullOrEmpty(xml))
            {
                test.Metadata.Add(MetadataKeys.XmlDocumentation, xml);
            }
        }

    //private static MSTest CreateMethodTest(ITypeInfo typeInfo, IMethodInfo methodInfo)
    //{
    //    MSTest methodTest = new MSTest(methodInfo.Name, methodInfo);
    //    methodTest.Kind = TestKinds.Test;
    //    methodTest.IsTestCase = true;

    //    PopulateTestMethodMetadata(methodInfo, methodTest);

    //    // Add XML documentation.
    //    string xmlDocumentation = methodInfo.GetXmlDocumentation();
    //    if (xmlDocumentation != null)
    //        methodTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);

    //    return methodTest;
    //}        

        //public override void ExploreAssembly(IAssemblyInfo assembly, Action<Test> consumer)
        //{
        //  Version frameworkVersion = GetFrameworkVersion(assembly);

        //  if (frameworkVersion != null)
        //  {
        //    Test frameworkTest = GetFrameworkTest(frameworkVersion, TestModel.RootTest);
        //    Test assemblyTest = GetAssemblyTest(assembly, frameworkTest, true);

        //    if (consumer != null)
        //      consumer(assemblyTest);
        //  }
        //}
        //private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        //{
        //  AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MachineSpecificationsAssemblyDisplayName);
        //  return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        //}

        //private Test GetFrameworkTest(Version frameworkVersion, Test rootTest)
        //{
        //  Test frameworkTest;
        //  if (!frameworkTests.TryGetValue(frameworkVersion, out frameworkTest))
        //  {
        //    frameworkTest = CreateFrameworkTest(frameworkVersion);
        //    rootTest.AddChild(frameworkTest);

        //    frameworkTests.Add(frameworkVersion, frameworkTest);
        //  }

        //  return frameworkTest;
        //}

        //private Test GetAssemblyTest(IAssemblyInfo assembly, Test frameworkTest, bool populateRecursively)
        //{
        //  Test assemblyTest;
        //  if (!assemblyTests.TryGetValue(assembly, out assemblyTest))
        //  {
        //    assemblyTest = CreateAssemblyTest(assembly);
        //    frameworkTest.AddChild(assemblyTest);

        //    assemblyTests.Add(assembly, assemblyTest);
        //  }

        //  if (populateRecursively)
        //  {
        //    PopulateAssemblyTest(assembly, assemblyTest);
        //  }

        //  return assemblyTest;
        //}

        //private void PopulateAssemblyTest(IAssemblyInfo assembly, Test assemblyTest)
        //{
        //  AssemblyExplorer explorer = new AssemblyExplorer();
        //  var specifications = explorer.FindContextsIn(assembly.Resolve());
        //  foreach (var specification in specifications)
        //  {
        //    var specificationTest = new MachineContextTest(specification);
        //    assemblyTest.AddChild(specificationTest);

        //    PopulateSpecificationTest(specification, specificationTest);
        //  }
        //}

        //private void PopulateSpecificationTest(Specifications.Model.Context context, MachineContextTest test)
        //{
        //  foreach (var specification in context.Specifications)
        //  {
        //    test.AddChild(new MachineSpecificationTest(specification));
        //  }
        //}

        //private static Test CreateFrameworkTest(Version frameworkVersion)
        //{
        //  BaseTest frameworkTest = new BaseTest(String.Format(Resources.MachineSpecificationExplorer_FrameworkNameWithVersionFormat, frameworkVersion), null);
        //  frameworkTest.BaselineLocalId = Resources.MachineSpecificationFramework_MachineSpecificationFrameworkName;
        //  frameworkTest.Kind = TestKinds.Framework;

        //  return frameworkTest;
        //}

        //private static Test CreateAssemblyTest(IAssemblyInfo assembly)
        //{
        //  BaseTest assemblyTest = new BaseTest(assembly.Name, assembly);
        //  assemblyTest.Kind = TestKinds.Assembly;

        //  ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

        //  return assemblyTest;
        //}
    }
}