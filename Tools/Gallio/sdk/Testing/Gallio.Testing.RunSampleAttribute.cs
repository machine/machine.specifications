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
    /// Used together with <see cref="BaseTestWithSampleRunner" /> to specify a sample fixture to run.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
    public class RunSampleAttribute : Attribute
    {
        private readonly Type fixtureType;
        private readonly string methodName;

        /// <summary>
        /// Specifies an explicit sample fixture to be run by the sample runner.
        /// </summary>
        /// <param name="fixtureType">The type of sample fixture.</param>
        public RunSampleAttribute(Type fixtureType)
            : this(fixtureType, String.Empty)
        {
        }

        /// <summary>
        /// Specifies an explicit sample fixture to be run by the sample runner.
        /// </summary>
        /// <param name="fixtureType">The type of sample fixture.</param>
        /// <param name="methodName">The name of the test method within the fixture.</param>
        public RunSampleAttribute(Type fixtureType, string methodName)
        {
            if (fixtureType == null)
                throw new ArgumentNullException("fixtureType");
            if (methodName == null)
                throw new ArgumentNullException("methodName");

            this.fixtureType = fixtureType;
            this.methodName = methodName;
        }

        /// <summary>
        /// Gets the type of the sample test fixture.
        /// </summary>
        public Type FixtureType
        {
            get
            {
                return fixtureType;
            }
        }

        /// <summary>
        /// Gets the name of the test method within the sample fixture.
        /// </summary>
        public string MethodName
        {
            get
            {
                return methodName;
            }
        }
    }
}