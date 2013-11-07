//-------------------------------------------------------------------------------
// <copyright file="MSpecTestMetadataExplorer.8.0.cs" company="bbv Software Services AG">
// Copyright (c) 2013
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Machine.Specifications.ReSharperRunner.Explorers
{
    using System.Threading;

    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.UnitTestFramework;

    public partial class MSpecTestMetadataExplorer
    {
        public void ExploreAssembly(
            IProject project,
            IMetadataAssembly assembly,
            UnitTestElementConsumer consumer,
            ManualResetEvent exitEvent)
        {
            this.ExploreAssembly(project, assembly, consumer);
        }
    }
}