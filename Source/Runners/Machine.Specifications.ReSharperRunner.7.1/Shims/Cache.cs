using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  [SolutionComponent]
  public class Cache : ICache
  {
    readonly CacheManager _cacheManager;

    public Cache(CacheManager cacheManager)
    {
      _cacheManager = cacheManager;
    }

    public IDeclarationsCache GetDeclarationsCache(IPsiModule psiModule, bool withReferences, bool caseSensitive)
    {
      return _cacheManager.GetDeclarationsCache(psiModule, withReferences, caseSensitive);
    }
  }
}