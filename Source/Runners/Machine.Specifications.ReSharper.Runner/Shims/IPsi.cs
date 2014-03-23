using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;

namespace Machine.Specifications.ReSharperRunner.Shims
{
  public interface IPsi
  {
    IPsiModule GetPrimaryPsiModule(IModule module);
  }
}