using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

namespace ObjectFactories.Services
{
  public interface IFactoryFinder
  {
    FactoryMap FindFactories(IAssembly assembly);
  }
}