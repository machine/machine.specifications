using System;
using System.Collections.Generic;

using Castle.Core;

namespace Castle.Facilities.DeferredServiceResolution
{
  public interface IServiceResolver
  {
    T Resolve<T>();
    ComponentModel ResolveModel(Type type);
    bool CanResolve(Type type);
  }
}
