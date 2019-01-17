using System;
using System.Security;

namespace Machine.Specifications.Runner.Utility
{
    internal class LongLivedMarshalByRefObject : MarshalByRefObject
    {
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}