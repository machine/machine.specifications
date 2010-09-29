using System;
using Machine.Specifications.Annotations;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
  public class BehaviorsAttribute : Attribute
  {
  }
}