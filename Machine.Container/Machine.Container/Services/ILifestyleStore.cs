using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface ILifestyleStore
  {
    ILifestyle ResolveLifestyle(ServiceEntry entry);
    void AddLifestyle(ServiceEntry entry, ILifestyle activator);
  }
}
