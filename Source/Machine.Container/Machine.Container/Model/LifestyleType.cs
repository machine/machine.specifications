using System;
using System.Collections.Generic;

namespace Machine.Container.Model
{
  public enum LifestyleType
  {
    Singleton,
    Transient
  }

  public abstract class LifestyleAttribute : Attribute
  {
    public abstract LifestyleType Lifestyle { get; }
  }
  
  [AttributeUsage(AttributeTargets.Class)]
  public class SingletonAttribute : LifestyleAttribute
  {
    public override LifestyleType Lifestyle
    {
      get { return LifestyleType.Singleton; }
    }
  }
  
  [AttributeUsage(AttributeTargets.Class)]
  public class TransientAttribute : LifestyleAttribute
  {
    public override LifestyleType Lifestyle
    {
      get { return LifestyleType.Transient; }
    }
  }
}
