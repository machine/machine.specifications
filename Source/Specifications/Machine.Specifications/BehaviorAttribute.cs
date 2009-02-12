using System;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public class BehaviorAttribute : Attribute
  {
  }
}