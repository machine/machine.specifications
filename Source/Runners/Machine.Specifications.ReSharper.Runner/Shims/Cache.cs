using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  [SolutionComponent]
  public class Cache : ICache
  {
    readonly ISymbolCache _cache;

    public Cache(ISymbolCache cache)
    {
      _cache = cache;
    }

    public ISymbolScope GetDeclarationsCache(IPsiModule psiModule, bool withReferences, bool caseSensitive)
    {
      return _cache.GetSymbolScope(LibrarySymbolScope.FULL, caseSensitive, psiModule.GetContextFromModule());
    }
  }
}