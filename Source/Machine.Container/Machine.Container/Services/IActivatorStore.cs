using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IActivatorStore
  {
    IActivator ResolveActivator(ServiceEntry entry);
    void AddActivator(ServiceEntry entry, IActivator activator);
    bool HasActivator(ServiceEntry entry);
  }
}
