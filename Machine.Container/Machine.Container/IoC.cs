using System;
using System.Collections.Generic;

using Machine.Container.Services;

namespace Machine.Container
{
  public static class IoC
  {
    #region Member Data
    private static IHighLevelContainer _container;
    #endregion

    #region Properties
    public static IHighLevelContainer Container
    {
      get
      {
        if (_container == null)
        {
          throw new ArgumentNullException();
        }
        return _container;
      }
      set
      {
        _container = value;
      }
    }
    #endregion
  }
}
