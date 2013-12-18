using System;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public class IgnoreAttribute : Attribute
  {
    [ObsoleteEx(Message = "Please specify the reason for ignoring the specification", RemoveInVersion = "0.8", TreatAsErrorFromVersion = "0.7")]
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