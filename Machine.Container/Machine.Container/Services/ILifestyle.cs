using System;
using System.Collections.Generic;

namespace Machine.Container.Services
{
  public interface ILifestyle : IActivator
  {
    void Initialize();
  }
}
