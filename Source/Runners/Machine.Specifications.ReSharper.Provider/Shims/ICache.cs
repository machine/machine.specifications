namespace Machine.Specifications.ReSharperProvider.Shims
{
    using JetBrains.ReSharper.Psi.Caches;
    using JetBrains.ReSharper.Psi.Modules;

    public interface ICache
    {
        ISymbolScope GetDeclarationsCache(IPsiModule psiModule, bool withReferences, bool caseSensitive);
    }
}