using System;

namespace Castle.Facilities.DeferredServiceResolution
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
  public class ManualWindsorRegistrationAttribute : Attribute
  {
  }
}