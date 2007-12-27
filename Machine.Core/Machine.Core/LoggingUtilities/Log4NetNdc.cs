using System;
using System.Collections.Generic;

namespace Machine.Core.LoggingUtilities
{
  public class Log4NetNdc : IDisposable
  {
    public static IDisposable Push(string message, params object[] objects)
    {
      return log4net.NDC.Push(String.Format(message, objects));
    }

    public Log4NetNdc(string message)
    {
      log4net.NDC.Push(message);
    }

    public void Dispose()
    {
      log4net.NDC.Pop();
    }
  }
}
