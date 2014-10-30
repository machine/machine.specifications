using System;
using System.Runtime.Serialization;

using Machine.Specifications;

namespace Example.CleanupFailure
{
  public class cleanup_failure
  {
    It is_inevitable = () => { };

    Cleanup after = () => { throw new NonSerializableException(); };
  }

  public class NonSerializableException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public NonSerializableException()
    {
    }

    public NonSerializableException(string message) : base(message)
    {
    }

    public NonSerializableException(string message, Exception inner) : base(message, inner)
    {
    }

    protected NonSerializableException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}