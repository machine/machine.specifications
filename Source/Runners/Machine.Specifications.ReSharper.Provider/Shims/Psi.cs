namespace Machine.Specifications.ReSharperProvider.Shims
{
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi.Modules;

    [SolutionComponent]
    public class Psi : IPsi
    {
        readonly IPsiModules _psiModules;

        public Psi(IPsiModules psiModules)
        {
            this._psiModules = psiModules;
        }

        public IPsiModule GetPrimaryPsiModule(IModule module)
        {
            return this._psiModules.GetPrimaryPsiModule(module);
        }
    }
}