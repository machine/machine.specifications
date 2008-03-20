using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Services;

namespace Machine.Container.Lifestyles
{
  public class TransientLifestyle : ILifestyle
  {
    #region Member Data
    private readonly IActivatorStrategy _activatorStrategy;
    private readonly ServiceEntry _serviceEntry;
    private IActivator _defaultActivator;
    #endregion

    #region TransientLifestyle()
    public TransientLifestyle(IActivatorStrategy activatorStrategy, ServiceEntry serviceEntry)
    {
      _activatorStrategy = activatorStrategy;
      _serviceEntry = serviceEntry;
    }
    #endregion

    #region ILifestyle Members
    public virtual void Initialize()
    {
      _defaultActivator = _activatorStrategy.CreateDefaultActivator(_serviceEntry);
    }

    public virtual bool CanActivate(ICreationServices services)
    {
      return _defaultActivator.CanActivate(services);
    }

    public virtual object Create(ICreationServices services)
    {
      return _defaultActivator.Create(services);
    }
    #endregion
  }
}
