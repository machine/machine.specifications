using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IDependencyResolver
  {
    IActivator ResolveDependency(ICreationServices services, ServiceEntry serviceEntry);
  }
}
