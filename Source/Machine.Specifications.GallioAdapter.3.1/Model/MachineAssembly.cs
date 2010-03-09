using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Machine.Specifications.Model;
using Gallio.Common.Reflection;

namespace Machine.Specifications.GallioAdapter.Model
{
    internal class MachineAssembly : MachineGallioTest
    {
        private readonly Version frameworkVersion;
        /// <summary>
        /// Creates an object to represent an MSpec assembly.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="codeElement">The point of definition, or null if none.</param>
        /// <param name="frameworkVersion">The version number of the MSpec.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, or
        /// <paramref name="frameworkVersion" /> is null.</exception>
        public MachineAssembly(string name, ICodeElementInfo codeElement, Version frameworkVersion) 
            : base(name, codeElement)
        {
            this.frameworkVersion = frameworkVersion;
        }

        /// <summary>
        /// Gets the path of the assembly.
        /// </summary>
        public string AssemblyFilePath
        {
            get { return ((IAssemblyInfo)CodeElement).Path; }
        }

        /// <summary>
        /// Gets the MSTest framework version.
        /// </summary>
        public Version FrameworkVersion
        {
            get { return frameworkVersion; }
        }
    }
}
