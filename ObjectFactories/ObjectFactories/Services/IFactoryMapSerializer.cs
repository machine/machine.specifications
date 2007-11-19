using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

namespace ObjectFactories.Services
{
  public interface IFactoryMapSerializer
  {
    void StoreFactoryMap(IAssembly assembly, FactoryMap factoryMap);
  }
}