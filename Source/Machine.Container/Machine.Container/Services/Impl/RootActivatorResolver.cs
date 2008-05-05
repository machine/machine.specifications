using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class RootActivatorResolver : IActivatorResolver
  {
    #region Member Data
    private readonly IActivatorResolver[] _resolvers;
    #endregion

    #region RootDependencyResolver()
    public RootActivatorResolver(params IActivatorResolver[] resolvers)
    {
      _resolvers = resolvers;
    }
    #endregion

    #region IActivatorResolver Members
    public IActivator ResolveActivator(ICreationServices services, ServiceEntry serviceEntry)
    {
      foreach (IActivatorResolver resolver in _resolvers)
      {
        IActivator activator = resolver.ResolveActivator(services, serviceEntry);
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
