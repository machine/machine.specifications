using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [MetadataUnitTestExplorer]
  public class MSpecTestMetadataExplorer : IUnitTestMetadataExplorer
  {
    readonly CacheManager _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly MSpecUnitTestProvider _provider;
    readonly PsiModuleManager _psiModuleManager;

    public MSpecTestMetadataExplorer(MSpecUnitTestProvider provider,
                                     IUnitTestElementManager manager,
                                     PsiModuleManager psiModuleManager,
                                     CacheManager cacheManager)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
    }

    public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
    {
      new AssemblyExplorer(_provider,
                           _manager,
                           _psiModuleManager,
                           _cacheManager,
                           assembly,
                           project,
                           consumer).Explore();
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}