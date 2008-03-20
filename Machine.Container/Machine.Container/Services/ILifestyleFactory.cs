using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface ILifestyleFactory
  {
    ILifestyle CreateSingletonLifestyle(ServiceEntry entry);
    ILifestyle CreateTransientLifestyle(ServiceEntry entry);
    ILifestyle CreateLifestyle(ServiceEntry entry);
  }
}
