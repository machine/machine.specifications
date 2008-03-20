using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IOverrideLookup
  {
    object LookupOverride(ServiceEntry serviceEntry);
  }
}
