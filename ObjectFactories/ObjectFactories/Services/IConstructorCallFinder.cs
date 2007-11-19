using System.Collections.Generic;
using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

namespace ObjectFactories.Services
{
  public interface IConstructorCallFinder
  {
    IEnumerable<ConstructorCallWeave> FindConstructorCallWeaves(IAssembly assembly, FactoryMap factories);
  }
}