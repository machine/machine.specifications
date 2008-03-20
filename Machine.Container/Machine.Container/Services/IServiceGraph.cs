using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IServiceGraph
  {
    ServiceEntry Lookup(Type type);
    void Add(ServiceEntry entry);
  }
}
