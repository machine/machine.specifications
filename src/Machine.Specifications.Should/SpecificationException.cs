using System;
using System.Runtime.Serialization;

namespace Machine.Specifications
{
    [Serializable]
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

        protected SpecificationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
