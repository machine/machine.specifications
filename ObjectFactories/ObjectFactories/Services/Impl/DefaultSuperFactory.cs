using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectFactories.Services.Impl
{
  public class DefaultSuperFactory : ISuperFactory
  {
    #region Member Data
    private readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
    private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
    #endregion

    #region DefaultSuperFactory()
    public DefaultSuperFactory()
    {
      Initialize(GetRuntimeFactoryInformation());
    }
    #endregion

    #region ISuperFactory Members
    public IObjectFactory<TType> CreateFactory<TType>()
    {
      Type objectType = typeof (TType);
      if (!_instances.ContainsKey(objectType))
      {
        throw new ObjectFactoryException("No factory type registered for {0}", objectType);
      }
      IObjectFactory<TType> factory = _instances[objectType] as IObjectFactory<TType>;
      if (factory == null)
      {
        throw new ObjectFactoryException("Type for factory was not IObjectFactory<TType>: {0}", _map[objectType]);
      }
      return factory;
    }
    #endregion

    #region Private Methods
    private static Type GetFactoryMapType()
    {
      string typeName = FactoryMapSerializer.FactoryMapTypeFullName;
      Type factoryMapType = Type.GetType(typeName, false);
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        factoryMapType = assembly.GetType(typeName);
        if (factoryMapType != null)
        {
          break;
        }
      }
      if (factoryMapType == null)
      {
        throw new ObjectFactoryException("{0} is missing, have you run the Weaver?", typeName);
      }
      return factoryMapType;
    }

    private static IRuntimeFactoryInformation GetRuntimeFactoryInformation()
    {
      Type factoryMapType = GetFactoryMapType();
      object instance = Activator.CreateInstance(factoryMapType);
      IRuntimeFactoryInformation factoryInformation = instance as IRuntimeFactoryInformation;
      if (factoryInformation == null)
      {
        throw new ObjectFactoryException("{0} should be a {1}", instance, typeof(IRuntimeFactoryInformation));
      }
      return factoryInformation;
    }

    private void Initialize(IRuntimeFactoryInformation runtimeFactoryInformation)
    {
      Type[][] allTypes = runtimeFactoryInformation.GetTypes();
      foreach (Type[] pair in allTypes)
      {
        _map[pair[0]] = pair[1];
        _instances[pair[0]] = Activator.CreateInstance(pair[1]);
      }
    }
    #endregion
  }
}