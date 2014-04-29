namespace Machine.Specifications.ReSharperProvider.Shims
{
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi.Caches;
    using JetBrains.ReSharper.Psi.Modules;

    [SolutionComponent]
    public class Cache : ICache
    {
        readonly ISymbolCache _cache;

        public Cache(ISymbolCache cache)
        {
            this._cache = cache;
        }

        public ISymbolScope GetDeclarationsCache(IPsiModule psiModule, bool withReferences, bool caseSensitive)
        {
            return this._cache.GetSymbolScope(LibrarySymbolScope.FULL, caseSensitive, psiModule.GetContextFromModule());
        }
    }
}