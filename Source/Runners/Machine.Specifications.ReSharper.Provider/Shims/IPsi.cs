namespace Machine.Specifications.ReSharperProvider.Shims
{
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi.Modules;

    public interface IPsi
    {
        IPsiModule GetPrimaryPsiModule(IModule module);
    }
}