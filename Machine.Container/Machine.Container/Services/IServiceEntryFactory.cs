using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IServiceEntryFactory
  {
    ServiceEntry CreateServiceEntry(Type serviceType, Type implementationType, LifestyleType lifestyleType);
  }
}
