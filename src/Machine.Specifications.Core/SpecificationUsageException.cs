using System;
using System.Runtime.Serialization;

namespace Machine.Specifications
{
#if !NET6_0_OR_GREATER
    [Serializable]
#endif
    public class SpecificationUsageException : Exception
    {
        public SpecificationUsageException()
        {
        }

        public SpecificationUsageException(string message)
            : base(message)
        {
        }

        public SpecificationUsageException(string message, Exception inner)
            : base(message, inner)
        {
        }

#if !NET6_0_OR_GREATER
        protected SpecificationUsageException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
