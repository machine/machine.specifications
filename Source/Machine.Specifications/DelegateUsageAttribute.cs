using System;

namespace Machine.Specifications
{
  [ObsoleteEx(Message = "DelegateUsageAttribute should be replaced by SetupDelegateAttribute, ActDelegateAttribute, BehaviorDelegateAttribute, AssertDelegateAttribute or CleanupDelegateAttribute accordingly.", RemoveInVersion = "0.8", TreatAsErrorFromVersion = "0.7")]
  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public class DelegateUsageAttribute : Attribute
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

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class SetupDelegateAttribute : DelegateUsageAttribute
  {
    public SetupDelegateAttribute() : base(DelegateUsage.Setup)
    {
    }
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class ActDelegateAttribute : DelegateUsageAttribute
  {
    public ActDelegateAttribute() : base(DelegateUsage.Act)
    {
    }
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class BehaviorDelegateAttribute : DelegateUsageAttribute
  {
    public BehaviorDelegateAttribute() : base(DelegateUsage.Behavior)
    {
    }
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class AssertDelegateAttribute : DelegateUsageAttribute
  {
    public AssertDelegateAttribute() : base(DelegateUsage.Assert)
    {
    }
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class CleanupDelegateAttribute : DelegateUsageAttribute
  {
    public CleanupDelegateAttribute() : base(DelegateUsage.Cleanup)
    {
    }
  }
}
