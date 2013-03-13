using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [FileUnitTestExplorer]
  public class MspecTestFileExplorer : IUnitTestFileExplorer
  {
    readonly CacheManager _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly MSpecUnitTestProvider _provider;
    readonly PsiModuleManager _psiModuleManager;

    public MspecTestFileExplorer(MSpecUnitTestProvider provider,
                                 IUnitTestElementManager manager,
                                 PsiModuleManager psiModuleManager,
                                 CacheManager cacheManager)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;

      _provider = provider;
    }

    public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
    {
      if ((psiFile.Language.Name == "CSHARP") || (psiFile.Language.Name == "VBASIC"))
      {
        psiFile.ProcessDescendants(new FileExplorer(_provider,
                                                    _manager,
                                                    _psiModuleManager,
                                                    _cacheManager,
                                                    consumer,
                                                    psiFile,
                                                    interrupted));
      }
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}