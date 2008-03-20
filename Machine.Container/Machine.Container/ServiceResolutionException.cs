using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Machine.Container
{
  [Serializable]
  [CoverageExclude]
  public class ServiceResolutionException : Exception
  {
    public ServiceResolutionException()
    {
    }

    public ServiceResolutionException(string message) : base(message)
    {
    }

    public ServiceResolutionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ServiceResolutionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }

  [Serializable]
  [CoverageExclude]
  public class CircularDependencyException : Exception
  {
    public CircularDependencyException()
    {
    }

    public CircularDependencyException(string message) : base(message)
    {
    }

    public CircularDependencyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CircularDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }

  [Serializable]
  [CoverageExclude]
  public class PendingDependencyException : Exception
  {
    public PendingDependencyException()
    {
    }

    public PendingDependencyException(string message) : base(message)
    {
    }

    public PendingDependencyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected PendingDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }

  [Serializable]
  [CoverageExclude]
  public class MissingServiceException : Exception
  {
    public MissingServiceException()
    {
    }

    public MissingServiceException(string message) : base(message)
    {
    }

    public MissingServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected MissingServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}