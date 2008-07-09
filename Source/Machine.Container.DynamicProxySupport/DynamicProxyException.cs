using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Machine.Container.DynamicProxySupport
{
  public class DynamicProxyException : ServiceContainerException
  {
    public DynamicProxyException(string message) : base(message)
    {
    }

    public DynamicProxyException()
    {
    }

    public DynamicProxyException(SerializationInfo info, StreamingContext context)
     : base(info, context)
    {
    }

    public DynamicProxyException(string message, Exception innerException)
     : base(message, innerException)
    {
    }
  }
}
