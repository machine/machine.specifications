using System;
using System.Collections.Generic;

namespace ObjectFactories.Services.Impl
{
  public class NullLogger : ILog
  {
    #region ILog Members
    public void Log(string format, params object[] arguments)
    {
      System.Diagnostics.Debug.WriteLine(String.Format(format, arguments));
    }
    #endregion
  }
}
