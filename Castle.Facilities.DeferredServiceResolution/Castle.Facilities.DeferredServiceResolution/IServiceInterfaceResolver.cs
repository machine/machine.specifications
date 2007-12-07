using System;
using System.Collections.Generic;

using Castle.Core;

namespace Castle.Facilities.DeferredServiceResolution
{
  public interface IServiceInterfaceResolver
  {
    ComponentModel AttemptResolve(Type serviceType, ICollection<ComponentModel> models);
    ComponentModel Resolve(Type serviceType, ICollection<ComponentModel> models);
  }
}