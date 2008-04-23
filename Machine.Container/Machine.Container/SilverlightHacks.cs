using System;

namespace log4net
{
  public interface ILog
  {
    void Info(string s);
  }

  public class Log : ILog
  {
    public void Info(string s)
    {
    }
  }

  public static class LogManager
  {
    public static ILog GetLogger(Type type)
    {
      return new Log();
    }
  }
}

namespace Machine
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SerializableAttribute : Attribute
  {
  }
}

  