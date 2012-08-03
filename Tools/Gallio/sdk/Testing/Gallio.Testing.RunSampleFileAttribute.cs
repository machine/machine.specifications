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
using Gallio.Framework.Pattern;

namespace Gallio.Testing
{
    /// <summary>
    /// Used together with <see cref="BaseTestWithSampleRunner" /> to specify a sample file to run.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
    public class RunSampleFileAttribute : Attribute
    {
        private readonly string filePath;

        /// <summary>
        /// Specifies an explicit sample fixture to be run by the sample runner.
        /// </summary>
        /// <param name="filePath">The path of sample test file.</param>
        public RunSampleFileAttribute(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            this.filePath = filePath;
        }

        /// <summary>
        /// Gets the path of the sample test file.
        /// </summary>
        public string FilePath
        {
            get
            {
                return filePath;
            }
        }
    }
}