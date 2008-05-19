using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Machine.Specifications
{
  [Serializable]
  public class SpecificationUsageException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public SpecificationUsageException()
    {
    }

    public SpecificationUsageException(string message) : base(message)
    {
    }

    public SpecificationUsageException(string message, Exception inner) : base(message, inner)
    {
    }

    protected SpecificationUsageException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }

  [Serializable]
  public class SpecificationVerificationException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public SpecificationVerificationException()
    {
    }

    public SpecificationVerificationException(string message) : base(message)
    {
    }

    public SpecificationVerificationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected SpecificationVerificationException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}
