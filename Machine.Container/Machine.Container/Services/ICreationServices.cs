using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface ICreationServices
  {
    IActivatorStore ActivatorStore
    {
      get;
    }

    IActivatorStrategy ActivatorStrategy
    {
      get;
    }

    IOverrideLookup Overrides
    {
      get;
    }

    Stack<ServiceEntry> Progress
    {
      get;
    }
  }
}
