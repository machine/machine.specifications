using System;
using System.Collections.Generic;

using Machine.Container.Services;

namespace Machine.Container.Activators
{
  public class LifestyleActivator : IActivator
  {
    #region Member Data
    private readonly ILifestyle _lifestyle;
    #endregion

    #region LifestyleActivator()
    public LifestyleActivator(ILifestyle lifestyle)
    {
      _lifestyle = lifestyle;
    }
    #endregion

    #region IActivator Members
    public bool CanActivate(ICreationServices services)
    {
      return _lifestyle.CanActivate(services);
    }

    public object Activate(ICreationServices services)
    {
      return _lifestyle.Activate(services);
    }
    #endregion
  }
}
