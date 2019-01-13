using System;

namespace Machine.Specifications
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
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