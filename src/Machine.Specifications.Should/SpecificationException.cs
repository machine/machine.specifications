using System;
using System.Runtime.Serialization;

namespace Machine.Specifications
{
#if !NET6_0_OR_GREATER
    [Serializable]
#endif
    public class SpecificationException : Exception
    {
        public SpecificationException()
        {
        }

        public SpecificationException(string message)
            : base(message)
        {
        }

        public SpecificationException(string message, Exception inner)
            : base(message, inner)
        {
        }

#if !NET6_0_OR_GREATER
        protected SpecificationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
