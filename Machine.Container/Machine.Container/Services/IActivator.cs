using System;
using System.Collections.Generic;

namespace Machine.Container.Services
{
  public interface IActivator
  {
    bool CanActivate(ICreationServices services);
    object Create(ICreationServices services);
  }
}