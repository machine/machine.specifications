using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IActivatorResolver
  {
    IActivator ResolveActivator(ICreationServices services, ServiceEntry serviceEntry);
  }
}
