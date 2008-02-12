using System;
using Castle.Core;

namespace Machine.IoC
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ProvidesTransientAttribute : ProvidesServiceAttribute
  {
    public ProvidesTransientAttribute(Type serviceType)
      : base(LifestyleType.Transient, serviceType)
    {
    }
  }
}