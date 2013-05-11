using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [MetadataUnitTestExplorer]
  public class MSpecTestMetadataExplorer : IUnitTestMetadataExplorer
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
      _assemblyExplorer.Explore(project, assembly, consumer);
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}