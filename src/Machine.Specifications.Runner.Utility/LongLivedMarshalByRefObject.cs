using System;
using System.Security;

namespace Machine.Specifications.Runner.Utility
{
#if !NETSTANDARD
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
