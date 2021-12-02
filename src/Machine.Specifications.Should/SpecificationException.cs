using System;
using System.Runtime.Serialization;

namespace Machine.Specifications
{
#if !NETSTANDARD
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

#if !NETSTANDARD
        protected SpecificationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
