using System;
using System.Collections.Generic;
using Machine.Container.Services;

namespace Machine.Container.Model
{
  public class ServiceEntry
  {
    #region Member Data
    private readonly List<ServiceEntry> _dependencies = new List<ServiceEntry>();
    private readonly Type _serviceType;
    private readonly Type _implementationType;
    private ConstructorCandidate _constructorCandidate;
    private bool _resolved;
    private LifestyleType _lifestyleType;
    private IActivator _activator;
    #endregion

    #region Properties
    public LifestyleType LifestyleType
    {
      get { return _lifestyleType; }
      set { _lifestyleType = value; }
    }

    public bool AreDependenciesResolved
    {
      get { return _resolved; }
      set { _resolved = value; }
    }
    
    public Type ServiceType
    {
      get { return _serviceType; }
    }

    public List<ServiceEntry> Dependencies
    {
      get { return _dependencies; }
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

    public IActivator Activator
    {
      get { return _activator; }
      set { _activator = value; }
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
      return String.Format("Entry<{0}, {1}, {2}>", this.ServiceType, this.ImplementationType, this.Dependencies.Count);
    }
    #endregion
  }
}