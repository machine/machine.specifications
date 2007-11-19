using System;
using System.Collections.Generic;

using ObjectFactories.CecilLayer;

namespace ObjectFactories.Services
{
  public interface IAssemblies
  {
    IAssembly LoadAssembly(string path);
    void SaveAssembly(IAssembly assembly, string path);
  }
}
