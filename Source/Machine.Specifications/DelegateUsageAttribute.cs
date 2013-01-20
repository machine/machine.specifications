using System;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class DelegateUsageAttribute : Attribute
  {
    public DelegateUsage DelegateUsage
    {
      get;
      private set;
    }

    public DelegateUsageAttribute(DelegateUsage delegateUsage)
    {
      DelegateUsage = delegateUsage;
    }
  }

  public enum DelegateUsage
  {
    Setup = 0,
    Act = 1,
    Behavior = 2,
    Assert = 3,
    Cleanup = 4
  }
}
