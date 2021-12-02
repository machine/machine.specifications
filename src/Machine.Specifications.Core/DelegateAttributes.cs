using System;

namespace Machine.Specifications
{
    [AttributeUsage(AttributeTargets.Delegate)]
    public sealed class SetupDelegateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Delegate)]
    public sealed class ActDelegateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Delegate)]
    public sealed class BehaviorDelegateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Delegate)]
    public sealed class AssertDelegateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Delegate)]
    public sealed class CleanupDelegateAttribute : Attribute
    {
    }
}
