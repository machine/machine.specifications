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
using System.Reflection;

namespace Gallio.Loader
{
    /// <summary>
    /// Gets version information for Gallio components.
    /// </summary>
    internal static class VersionPolicy
    {
        /// <summary>
        /// Generates a version label from a version number for display purposes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A Gallio version label consists of a major version, minor version and build number and
        /// is of the form "3.1 build 456".  The revision number is not displayed.
        /// </para>
        /// <para>
        /// Gallio component assemblies contain two different version numbers: an assembly version number
        /// and a file version number.  To preserve assembly compatibility across builds of the same version,
        /// the assembly version number omits build number and revision information.  Consequently
        /// <see cref="GetVersionLabel(System.Reflection.Assembly)" /> derived the version label from the file version number.
        /// </para>
        /// </remarks>
        /// <param name="version">The version to transform into a label.</param>
        /// <returns>The version label.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="version"/> is null.</exception>
        public static string GetVersionLabel(Version version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            return String.Format("{0}.{1} build {2}", version.Major, version.Minor, version.Build);
        }

        /// <summary>
        /// Gets a version label of a Gallio component assembly for display purposes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A Gallio version label consists of a major version, minor version and build number and
        /// is of the form "3.1 build 456".  The revision number is not displayed.
        /// </para>
        /// <para>
        /// Gallio component assemblies contain two different version numbers: an assembly version number
        /// and a file version number.  To preserve assembly compatibility across builds of the same version,
        /// the assembly version number omits build number and revision information.  Consequently
        /// <see cref="GetVersionLabel(System.Reflection.Assembly)" /> derived the version label from the file version number.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The version label.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static string GetVersionLabel(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return GetVersionLabel(GetVersionNumber(assembly));
        }

        /// <summary>
        /// Gets the version number of an assembly for display purposes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns the file version (<see cref="AssemblyFileVersionAttribute" />) when available,
        /// otherwise returns the assembly version.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The version number.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static Version GetVersionNumber(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");

            var attribs = (AssemblyFileVersionAttribute[]) assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attribs.Length != 0)
                return new Version(attribs[0].Version);

            return assembly.GetName().Version;
        }
    }
}
