using System;
using System.Security;

namespace Machine.Specifications.Runner.Utility
{
#if !NET6_0_OR_GREATER
    internal class LongLivedMarshalByRefObject : MarshalByRefObject
    {
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
#else
    internal class LongLivedMarshalByRefObject
    {
    }
#endif
}
