using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace ObjectFactories.Model
{
  public class Factory
  {
    private readonly TypeReference _objectType;
    private readonly TypeDefinition _factoryType;

    public TypeReference ObjectType
    {
      get { return _objectType; }
    }

    public TypeDefinition FactoryType
    {
      get { return _factoryType; }
    }

    public Factory(TypeReference objectType, TypeDefinition factoryType)
    {
      _objectType = objectType;
      _factoryType = factoryType;
    }
  }
}
