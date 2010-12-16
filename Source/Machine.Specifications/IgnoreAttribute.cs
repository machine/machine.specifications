using System;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public class IgnoreAttribute : Attribute
  {
    [Obsolete("Please specify the reason for ignoring the specification")]
    public IgnoreAttribute()
    {
    }

    public IgnoreAttribute(string reason)
    {
      Reason = reason;
    }

    public string Reason
    {
      get;
      private set;
    }
  }
}