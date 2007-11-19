using System;
using System.Collections.Generic;

namespace ObjectFactories.Services
{
  public interface ILog
  {
    void Log(string format, params object[] arguments);
  }
}
