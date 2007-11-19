using System;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class ServiceResolutionException : Exception
  {
    public ServiceResolutionException(string message)
      : base(message)
    {
    }
    public ServiceResolutionException(string message, Exception innerException)
     : base(message, innerException)
    {
    }
  }
}