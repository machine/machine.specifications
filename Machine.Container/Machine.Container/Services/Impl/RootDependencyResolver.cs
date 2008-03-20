using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class RootDependencyResolver : IDependencyResolver
  {
    #region Member Data
    private readonly IDependencyResolver[] _resolvers;
    #endregion

    #region RootDependencyResolver()
    public RootDependencyResolver(params IDependencyResolver[] resolvers)
    {
      _resolvers = resolvers;
    }
    #endregion

    #region IDependencyResolver Members
    public IActivator ResolveDependency(ICreationServices services, ServiceEntry serviceEntry)
    {
      foreach (IDependencyResolver resolver in _resolvers)
      {
        IActivator activator = resolver.ResolveDependency(services, serviceEntry);
        if (activator != null)
        {
          return activator;
        }
      }
      return null;
    }
    #endregion
  }
}
