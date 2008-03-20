using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ThrowsPendingActivatorResolver : IActivatorResolver
  {
    #region IActivatorResolver Members
    public IActivator ResolveActivator(ICreationServices services, ServiceEntry serviceEntry)
    {
      throw new PendingDependencyException(new ResolutionMessageBuilder(serviceEntry, services.Progress).ToString());
    }
    #endregion
  }
}
