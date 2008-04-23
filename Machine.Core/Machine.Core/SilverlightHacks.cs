using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Castle.Core
{
  public enum LifestyleType
  {
    Transient,
    Singleton
  }

  public abstract class LifestyleAttribute : Attribute
  {
    public abstract LifestyleType Lifestyle { get; }
  }
  
  [AttributeUsage(AttributeTargets.Class)]
  public class SingletonAttribute : Attribute
  {
    public LifestyleType Lifestyle
    {
      get { return LifestyleType.Singleton; }
    }
  }
  
  [AttributeUsage(AttributeTargets.Class)]
  public class TransientAttribute : Attribute
  {
    public LifestyleType Lifestyle
    {
      get { return LifestyleType.Transient; }
    }
  }
}