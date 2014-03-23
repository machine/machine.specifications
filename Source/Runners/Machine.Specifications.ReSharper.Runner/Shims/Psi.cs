using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  [SolutionComponent]
  public class Psi : IPsi
  {
    readonly IPsiModules _psiModules;

    public Psi(IPsiModules psiModules)
    {
      _psiModules = psiModules;
    }

    public IPsiModule GetPrimaryPsiModule(IModule module)
    {
      return _psiModules.GetPrimaryPsiModule(module);
    }
  }
}