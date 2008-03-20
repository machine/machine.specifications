using System;
using System.Collections.Generic;
using System.Diagnostics;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class DotNetObjectFactory : IObjectFactory
  {
    #region IObjectFactory Members
    public virtual object CreateObject(ConstructorCandidate constructor, object[] parameters)
    {
      return constructor.RuntimeInfo.Invoke(parameters);
    }
    #endregion
  }
}