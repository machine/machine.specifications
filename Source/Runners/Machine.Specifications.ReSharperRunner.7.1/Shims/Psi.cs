using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  [SolutionComponent]
  class Psi : IPsi
  {
    readonly PsiModuleManager _psiModuleManager;

    public Psi(PsiModuleManager psiModuleManager)
    {
      _psiModuleManager = psiModuleManager;
    }

    public IPsiModule GetPrimaryPsiModule(IModule module)
    {
      return _psiModuleManager.GetPrimaryPsiModule(module);
    }
  }
}