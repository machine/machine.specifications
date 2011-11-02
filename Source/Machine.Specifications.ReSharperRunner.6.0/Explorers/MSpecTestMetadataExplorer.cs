using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [MetadataUnitTestExplorer]
  public class MSpecTestMetadataExplorer : IUnitTestMetadataExplorer
  {
    readonly MSpecUnitTestProvider _provider;
#if RESHARPER_61
    readonly IUnitTestElementManager _manager;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;
#endif

#if RESHARPER_61
    public MSpecTestMetadataExplorer(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public MSpecTestMetadataExplorer(MSpecUnitTestProvider provider)
    {
#endif
      _provider = provider;
    }

    public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
    {
      new AssemblyExplorer(_provider,
#if RESHARPER_61
        _manager, _psiModuleManager, _cacheManager, 
#endif
        assembly, project, consumer).Explore();
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}