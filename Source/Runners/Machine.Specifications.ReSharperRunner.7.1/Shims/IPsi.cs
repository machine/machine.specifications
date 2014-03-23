using JetBrains.ProjectModel;
#if RESHARPER_8
using JetBrains.ReSharper.Psi.Modules;
#else
using JetBrains.ReSharper.Psi;
#endif

namespace Machine.Specifications.ReSharperRunner.Shims
{
  public interface IPsi
  {
    IPsiModule GetPrimaryPsiModule(IModule module);
  }
}