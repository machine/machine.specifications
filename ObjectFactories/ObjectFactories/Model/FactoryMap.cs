using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace ObjectFactories.Model
{
  public class FactoryMap
  {
    private readonly List<Factory> _factories = new List<Factory>();
    private readonly Dictionary<TypeReference, Factory> _factoriesByObjectType = new Dictionary<TypeReference, Factory>();
    private readonly Dictionary<TypeReference, Factory> _factoriesByFactoryType = new Dictionary<TypeReference, Factory>();

    public IEnumerable<Factory> Factories
    {
      get { return _factories;  }
    }

    public Factory AddFactory(TypeReference objectType, TypeDefinition factoryType)
    {
      if (_factoriesByObjectType.ContainsKey(objectType))
      {
        throw new ArgumentException("objectType");
      }
      Factory factory = new Factory(objectType, factoryType);
      _factories.Add(factory);
      _factoriesByFactoryType[factory.FactoryType] = factory;
      _factoriesByObjectType[factory.ObjectType] = factory;
      return factory;
    }

    public Factory GetForFactoryType(TypeReference factoryType)
    {
      return _factoriesByFactoryType[factoryType];
    }

    public bool HasForFactoryType(TypeReference factoryType)
    {
      return _factoriesByFactoryType.ContainsKey(factoryType);
    }

    public Factory GetForObjectType(TypeReference objectType)
    {
      return _factoriesByObjectType[objectType];
    }

    public bool HasForObjectType(TypeReference objectType)
    {
      return _factoriesByObjectType.ContainsKey(objectType);
    }
  }
}
