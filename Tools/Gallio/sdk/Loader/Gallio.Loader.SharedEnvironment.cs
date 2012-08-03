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
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Gallio.Loader.Isolation;

namespace Gallio.Loader.SharedEnvironment
{
    /// <summary>
    /// The Gallio.Loader.SharedEnvironment namespace makes it easy to share
    /// a single isolated environment across the whole process.
    /// </summary>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Manages a shared instance of an isolated Gallio runtime environment.
    /// The environment is configured to have access to the host's own assemblies
    /// in addition to Gallio's.
    /// </summary>
    internal static class SharedEnvironmentManager
    {
        private static readonly object sharedEnvironmentSyncRoot = new object();
        private static IIsolatedEnvironment sharedEnvironment;

        /// <summary>
        /// Gets the shared environment.
        /// </summary>
        /// <returns>The shared environment.</returns>
        public static IIsolatedEnvironment GetSharedEnvironment()
        {
            lock (sharedEnvironmentSyncRoot)
            {
                if (sharedEnvironment == null)
                    sharedEnvironment = CreateSharedEnvironment();

                return sharedEnvironment;
            }
        }

        private static IIsolatedEnvironment CreateSharedEnvironment()
        {
            IIsolatedEnvironment environment = IsolatedEnvironmentManager.CreateIsolatedEnvironment();
            environment.Loader.AddHintDirectory(LoaderUtils.GetLoaderDirectoryPath());
            environment.Loader.SetupRuntime();
            return environment;
        }
    }
}