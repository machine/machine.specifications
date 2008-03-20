using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IActivatorStrategy
  {
    IActivator CreateLifestyleActivator(ILifestyle lifestyle);
    IActivator CreateStaticActivator(ServiceEntry entry, object instance);
    IActivator CreateDefaultActivator(ServiceEntry entry);
  }
}