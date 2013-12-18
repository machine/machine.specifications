using System;
using System.Runtime.Serialization;

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

      protected SpecificationException(
        SerializationInfo info,
        StreamingContext context)
          : base(info, context)
      {
      }
  }
}