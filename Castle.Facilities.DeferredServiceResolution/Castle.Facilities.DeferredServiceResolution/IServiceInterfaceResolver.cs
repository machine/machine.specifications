using System;
using System.Collections.Generic;

using Castle.Core;

namespace Castle.Facilities.DeferredServiceResolution
{
  public interface IServiceInterfaceResolver
  {
    ComponentModel Resolve(Type serviceType, ICollection<ComponentModel> models, bool throwOnError);
    ComponentModel Resolve(Type serviceType, ICollection<ComponentModel> models);
  }
}