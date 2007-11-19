using System.Collections.Generic;
using ObjectFactories.Model;

namespace ObjectFactories.Services
{
  public interface IFactoryCallWeaver
  {
    void WeaveConstructorCalls(IEnumerable<ConstructorCallWeave> weaves, FactoryMap factories);
  }
}