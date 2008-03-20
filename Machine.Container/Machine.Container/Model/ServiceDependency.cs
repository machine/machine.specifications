using System;
using System.Collections.Generic;

namespace Machine.Container.Model
{
  public class ServiceDependency
  {
    #region Member Data
    private readonly Type _dependencyType;
    private readonly DependencyType _type;
    #endregion

    #region Properties
    public Type DependencyType
    {
      get { return _dependencyType; }
    }
    #endregion

    #region ServiceDependency()
    public ServiceDependency(Type dependencyType, DependencyType type)
    {
      _dependencyType = dependencyType;
      _type = type;
    }
    #endregion
  }
}