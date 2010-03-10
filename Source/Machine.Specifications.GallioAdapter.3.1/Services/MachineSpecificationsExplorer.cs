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
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Machine.Specifications.Explorers;
using Machine.Specifications.Utility;
using Machine.Specifications.GallioAdapter.Model;

namespace Machine.Specifications.GallioAdapter.Services
{
    public class MachineSpecificationsExplorer : TestExplorer
    {
        private const string MachineSpecificationsAssemblyDisplayName = @"Machine.Specifications";
        
        private readonly Dictionary<IAssemblyInfo, MachineAssembly> assemblyTests;
        private readonly Dictionary<ITypeInfo, Test> typeTests;

        public MachineSpecificationsExplorer()
        {
            assemblyTests = new Dictionary<IAssemblyInfo, MachineAssembly>();
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
            MachineAssembly assemblyTest;
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
                Assembly resolvedAssembly = assembly.Resolve(false);
                var contexts = explorer.FindContextsIn(resolvedAssembly);                

                assemblyTest.Contexts = explorer.FindAssemblyContextsIn( resolvedAssembly).ToList();                

                foreach (var context in contexts)
                {
                    MachineContextTest contextTest = new MachineContextTest(context);

                    context.Specifications.Each(spec => AddSpecification(contextTest, spec));

                    AddContextMetadata(contextTest, context);                                          
                    assemblyTest.AddChild(contextTest);                                        
                }                
            }

            return assemblyTest;
        }        

        private static MachineAssembly CreateAssemblyTest(IAssemblyInfo assembly, Version frameworkVersion)
        {
            MachineAssembly assemblyTest = new MachineAssembly(assembly.Name, assembly, frameworkVersion);
            assemblyTest.Kind = TestKinds.Assembly;

            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

            return assemblyTest;
        }

        private void AddContextMetadata(MachineContextTest contextTest, Machine.Specifications.Model.Context context)
        {
            if (context.Subject != null) 
                contextTest.Metadata.Add(MetadataKeys.Category, context.Subject.FullConcern);
            
            if( context.Tags != null && context.Tags.Any())
                contextTest.Metadata.Add(SpecificationMetadataKeys.Tags, context.Tags.Select( t => t.Name).ToList());

            if (context.IsIgnored)
                contextTest.Metadata.Add(MetadataKeys.IgnoreReason, "The context has the IgnoreAttribute");            

            AddXmlComment(contextTest, Reflector.Wrap(context.Type));
        }

        private void AddSpecification(MachineContextTest parent, Machine.Specifications.Model.Specification specification)
        {            
            MachineSpecificationTest specificationTest = new MachineSpecificationTest( specification);

            if (specification.IsIgnored)
            {
                string reason = parent.Context.IsIgnored ? "The parent context has the IgnoreAttribute" : "The specification has the IgnoreAttribute";
                specificationTest.Metadata.Add(MetadataKeys.IgnoreReason, reason);
            }

            AddXmlComment(specificationTest, Reflector.Wrap(specification.FieldInfo));

            parent.AddChild(specificationTest);
        }

        private void AddXmlComment(Test test, ICodeElementInfo element)
        {
            string xml = element.GetXmlDocumentation();
            if (!string.IsNullOrEmpty(xml))
            {
                test.Metadata.Add(MetadataKeys.XmlDocumentation, xml);
            }
        }    
    }
}