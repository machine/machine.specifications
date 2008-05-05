using System;
using System.Collections.Generic;

namespace Machine.Container.Model
{
  public class ServiceRegistration
  {
    private readonly Type _serviceType;
    private readonly Type _implementationType;

    public Type ServiceType
    {
      get { return _serviceType; }
    }

    public Type ImplementationType
    {
      get { return _implementationType; }
    }

    public ServiceRegistration(Type serviceType, Type implementationType)
    {
      _serviceType = serviceType;
      _implementationType = implementationType;
    }

    public override string ToString()
    {
      return String.Format("ServiceRegistration<{0}, {1}>", _serviceType, _implementationType);
    }
  }
}
