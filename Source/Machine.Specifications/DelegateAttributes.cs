using System;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class SetupDelegateAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class ActDelegateAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class BehaviorDelegateAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class AssertDelegateAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
  public sealed class CleanupDelegateAttribute : Attribute
  {
  }
}
