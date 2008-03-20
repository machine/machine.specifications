using System;
using System.Collections.Generic;
using System.Reflection;

namespace Machine.Container.Model
{
  public class ConstructorCandidate
  {
    #region Member Data
    private readonly List<ServiceDependency> _dependencies = new List<ServiceDependency>();
    private readonly ConstructorInfo _runtimeInfo;
    #endregion

    #region Properties
    public List<ServiceDependency> Dependencies
    {
      get { return _dependencies; }
    }

    public ConstructorInfo RuntimeInfo
    {
      get { return _runtimeInfo; }
    }
    #endregion

    #region ConstructorCandidate()
    public ConstructorCandidate(ConstructorInfo runtimeInfo)
    {
      _runtimeInfo = runtimeInfo;
    }
    #endregion
  }
}