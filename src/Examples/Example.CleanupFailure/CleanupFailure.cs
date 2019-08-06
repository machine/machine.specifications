using System;
using Machine.Specifications;

namespace Example.CleanupFailure
{
    public class cleanup_failure
    {
        It is_inevitable = () => { };

        Cleanup after = () =>
            throw new NonSerializableException();
    }

    public class NonSerializableException : Exception
    {
    }
}
