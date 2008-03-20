using System;
using System.Collections.Generic;

using Machine.Container.Services;

namespace Machine.Container.Model
{
  public class ServiceEntry
  {
    #region Member Data
    private readonly Type _serviceType;
    private readonly Type _implementationType;
    private ConstructorCandidate _constructorCandidate;
    private LifestyleType _lifestyleType;
    #endregion

    #region Properties
    public LifestyleType LifestyleType
    {
      get { return _lifestyleType; }
      set { _lifestyleType = value; }
    }

    public Type ServiceType
    {
      get { return _serviceType; }
    }

    public Type ImplementationType
    {
      get { return _implementationType; }
    }

    public Type ConcreteType
    {
      get
      {
        if (_implementationType != null && !_implementationType.IsAbstract)
        {
          return _implementationType;
        }
        if (!_serviceType.IsAbstract)
        {
          return _serviceType;
        }
        return null;
      }
    }

    public ConstructorCandidate ConstructorCandidate
    {
      get { return _constructorCandidate; }
      set { _constructorCandidate = value; }
    }
    #endregion

    #region ServiceEntry()
    public ServiceEntry(Type serviceType, Type implementationType, LifestyleType lifestyleType)
    {
      _serviceType = serviceType;
      _implementationType = implementationType;
      _lifestyleType = lifestyleType;
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return String.Format("Entry<{0}, {1}>", this.ServiceType, this.ImplementationType);
    }
    #endregion
  }
}