using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  public interface ICache
  {
    ISymbolScope GetDeclarationsCache(IPsiModule psiModule, bool withReferences, bool caseSensitive);
  }
}