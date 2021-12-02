using System;
using JetBrains.Annotations;

namespace Machine.Specifications
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    public class BehaviorsAttribute : Attribute
    {
    }
}
