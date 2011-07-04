using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [MetadataUnitTestExplorer]
  public class MSpecTestMetadataExplorer : IUnitTestMetadataExplorer
  {
    readonly MSpecUnitTestProvider _provider;

    public MSpecTestMetadataExplorer(MSpecUnitTestProvider provider)
    {
      _provider = provider;
    }

    public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
    {
      new AssemblyExplorer(_provider, assembly, project, consumer).Explore();
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}