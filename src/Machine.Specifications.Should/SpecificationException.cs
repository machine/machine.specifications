using System;
using System.Runtime.Serialization;

namespace Machine.Specifications
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class SpecificationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

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
