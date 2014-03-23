using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  public interface ICache
  {
    IDeclarationsCache GetDeclarationsCache(IPsiModule psiModule, bool withReferences, bool caseSensitive);
  }
}