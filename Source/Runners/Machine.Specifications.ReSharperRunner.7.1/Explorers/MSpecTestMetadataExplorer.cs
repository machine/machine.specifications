using System.Collections.Generic;
using System.Linq;

using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
    [MetadataUnitTestExplorer]
    public partial class MSpecTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        readonly AssemblyExplorer _assemblyExplorer;
        readonly MSpecUnitTestProvider _provider;

        public MSpecTestMetadataExplorer(MSpecUnitTestProvider provider,
                                         AssemblyExplorer assemblyExplorer)
        {
            _assemblyExplorer = assemblyExplorer;
            _provider = provider;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
        {
            using (ReadLockCookie.Create()) //Get a read lock so that it is safe to read the assembly
            {
                foreach (var metadataTypeInfo in GetTypesIncludingNested(assembly.GetTypes()))
                    this._assemblyExplorer.Explore(project, assembly, consumer, metadataTypeInfo);
            }
        }

        private static IEnumerable<IMetadataTypeInfo> GetTypesIncludingNested(IEnumerable<IMetadataTypeInfo> types)
        {
            foreach (var type in (types ?? Enumerable.Empty<IMetadataTypeInfo>()))
            {
                foreach (var nestedType in GetTypesIncludingNested(type.GetNestedTypes())) //getting nested classes too
                {
                    yield return nestedType;
                }

                yield return type;
            }
        }

        public IUnitTestProvider Provider
        {
            get { return _provider; }
        }
    }
}