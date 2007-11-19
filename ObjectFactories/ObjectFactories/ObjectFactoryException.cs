using System;
using System.Collections.Generic;

namespace ObjectFactories
{
  public class ObjectFactoryException : Exception
  {
    public ObjectFactoryException()
    {
    }

    public ObjectFactoryException(string message, params object[] messageArguments)
     : base(String.Format(message, messageArguments))
    {
    }

    public ObjectFactoryException(Exception innerException, string message, params object[] messageArguments)
     : base(String.Format(message, messageArguments), innerException)
    {
    }
  }
}
