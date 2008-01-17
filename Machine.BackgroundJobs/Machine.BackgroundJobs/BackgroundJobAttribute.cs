using System;
using System.Collections.Generic;

using Machine.BackgroundJobs.Services;

namespace Machine.BackgroundJobs
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
  public class BackgroundJobAttribute : Attribute
  {
    private readonly Type _handlerType;
    private readonly Type _repositoryType;

    public Type HandlerType
    {
      get { return _handlerType; }
    }

    public Type RepositoryType
    {
      get { return _repositoryType; }
    }

    public BackgroundJobAttribute(Type handlerType, Type repositoryType)
    {
      if (!typeof(IBackgroundJobHandler).IsAssignableFrom(handlerType)) throw new ArgumentException("handlerType");
      if (!typeof(IJobRepository).IsAssignableFrom(repositoryType)) throw new ArgumentException("repositoryType");
      _handlerType = handlerType;
      _repositoryType = repositoryType;
    }
  }
}
