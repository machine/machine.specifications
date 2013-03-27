using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  public class Cache : ICache
  {
    readonly ISymbolCache _cache;

    public Cache(ISymbolCache cache)
    {
      _cache = cache;
    }

    public IDeclarationsCache GetDeclarationsCache(IPsiModule psiModule, bool withReferences, bool caseSensitive)
    {
      throw new System.NotImplementedException();
    }
  }
}