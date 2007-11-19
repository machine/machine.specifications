using System;
using System.Collections.Generic;

using Mono.Cecil;

using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

namespace ObjectFactories.Services.Impl
{
  public class FactoryFinder : IFactoryFinder
  {
    #region Member Data
    private readonly ILog _log;
    #endregion

    #region FactoryFinder()
    public FactoryFinder(ILog log)
    {
      _log = log;
    }
    #endregion

    #region IFactoryFinder Members
    public FactoryMap FindFactories(IAssembly assembly)
    {
      FactoryMap map = new FactoryMap();
      foreach (TypeDefinition type in assembly.Types)
      {
        TypeReference objectIsFactoryFor = IsFactory(type);
        if (objectIsFactoryFor != null)
        {
          _log.Log("Found {0} as factory for {1}", type.FullName, objectIsFactoryFor.FullName);
          map.AddFactory(objectIsFactoryFor, type);
        }
      }
      return map;
    }

    public static TypeReference IsFactory(TypeDefinition type)
    {
      Type iObjectFactory = typeof(IObjectFactory<>);
      foreach (TypeReference interfaceType in type.Interfaces)
      {
        if (interfaceType.Namespace == iObjectFactory.Namespace &&
            interfaceType.Name == iObjectFactory.Name)
        {
          GenericInstanceType genericInterface = (GenericInstanceType)interfaceType;
          return genericInterface.GenericArguments[0];
        }
      }
      return null;
    }
    #endregion
  }
}
