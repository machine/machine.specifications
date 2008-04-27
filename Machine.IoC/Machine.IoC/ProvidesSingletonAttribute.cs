using System;
using System.Collections.Generic;

using Castle.Core;

namespace Machine.WindsorExtensions
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ProvidesSingletonAttribute : ProvidesServiceAttribute
  {
    public ProvidesSingletonAttribute(Type serviceType)
      : base(LifestyleType.Singleton, serviceType)
    {
    }
  }
}