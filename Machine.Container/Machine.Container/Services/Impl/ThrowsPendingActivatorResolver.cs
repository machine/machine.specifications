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
      throw new PendingDependencyException(ResolutionMessageBuilder.BuildMessage(serviceEntry, services.Progress));
    }
    #endregion
  }
}
