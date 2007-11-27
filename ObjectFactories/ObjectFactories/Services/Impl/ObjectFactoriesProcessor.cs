using System;
using System.Collections.Generic;

using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

namespace ObjectFactories.Services.Impl
{
  public class ObjectFactoriesProcessor : IObjectFactoriesProcessor
  {
    #region Member Data
    private readonly ILog _log;
    private readonly IAssemblies _assemblies;
    private readonly IFactoryFinder _factoryFinder;
    private readonly IConstructorCallFinder _constructorCallFinder;
    private readonly IFactoryCallWeaver _factoryCallWeaver;
    private readonly IFactoryMapSerializer _factoryMapSerializer;
    #endregion

    #region ObjectFactoriesProcessor()
    public ObjectFactoriesProcessor(ILog log, IAssemblies assemblies, IFactoryFinder factoryFinder, IConstructorCallFinder constructorCallFinder, IFactoryCallWeaver factoryCallWeaver, IFactoryMapSerializer factoryMapSerializer)
    {
      _log = log;
      _factoryMapSerializer = factoryMapSerializer;
      _assemblies = assemblies;
      _factoryCallWeaver = factoryCallWeaver;
      _constructorCallFinder = constructorCallFinder;
      _factoryFinder = factoryFinder;
    }
    #endregion

    #region IObjectFactoriesProcessor Members
    public void ProcessAssembly(string path)
    {
      IAssembly assembly = _assemblies.LoadAssembly(path);
      EnumerableFinder enumerableFinder = new EnumerableFinder(_log);
      enumerableFinder.FindEnumerables(assembly);
      FactoryMap factories = _factoryFinder.FindFactories(assembly);
      IEnumerable<ConstructorCallWeave> weaves = _constructorCallFinder.FindConstructorCallWeaves(assembly, factories);
      _factoryCallWeaver.WeaveConstructorCalls(weaves, factories);
      _factoryMapSerializer.StoreFactoryMap(assembly, factories);
      assembly.Save(path);
    }
    #endregion
  }
}
