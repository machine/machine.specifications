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
using System.Linq;
using System.Text;
using Gallio.Model;
using Machine.Specifications.Model;
using Gallio.Common.Reflection;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>
  /// Represents an assembly with Machine Specifications
  /// </summary>
  /// <remarks>
  /// Adapted from the MS Test Adaptor of the Gallio Project
  /// </remarks>
  internal class MachineAssemblyTest : MachineGallioTest
  {
    readonly Version _frameworkVersion;

    public IList<IAssemblyContext> AssemblyContexts { get; set; }
    public IList<ICleanupAfterEveryContextInAssembly> GlobalCleanup { get; set; }
    public IList<ISupplementSpecificationResults> SpecificationSupplements { get; set; }           
    
    public string AssemblyFilePath 
    {
      get { return ((IAssemblyInfo)CodeElement).Path; } 
    }
    
    public Version FrameworkVersion 
    { 
      get { return _frameworkVersion; } 
    }    

    /// <summary>
    /// Creates an object to represent an MSpec assembly.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <param name="codeElement">The point of definition, or null if none.</param>
    /// <param name="frameworkVersion">The version number of the MSpec.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, or
    /// <paramref name="frameworkVersion" /> is null.</exception>
    public MachineAssemblyTest(string name, ICodeElementInfo codeElement, Version frameworkVersion) 
      : base(name, codeElement)
    {
      _frameworkVersion = frameworkVersion;
      AssemblyContexts = new List<IAssemblyContext>();
    }    
  }
}
