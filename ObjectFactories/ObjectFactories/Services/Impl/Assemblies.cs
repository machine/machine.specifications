using System;
using System.Collections.Generic;

using Mono.Cecil;

using ObjectFactories.CecilLayer;

namespace ObjectFactories.Services.Impl
{
  public class Assemblies : IAssemblies
  {
    #region IAssemblies Members
    public IAssembly LoadAssembly(string path)
    {
      return new AssemblyWrapper(AssemblyFactory.GetAssembly(path));
    }

    public void SaveAssembly(IAssembly assembly, string path)
    {
      assembly.Save(path);
    }
    #endregion
  }
}
