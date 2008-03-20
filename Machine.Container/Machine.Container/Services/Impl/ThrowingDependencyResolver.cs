using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ThrowingDependencyResolver : IDependencyResolver
  {
    #region IDependencyResolver Members
    public IActivator ResolveDependency(ICreationServices services, ServiceEntry serviceEntry)
    {
      throw new PendingDependencyException(new ResolutionMessageBuilder(serviceEntry, services.Progress).ToString());
    }
    #endregion
  }
}
