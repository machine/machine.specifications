using System;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Example.Random
{
    public class AsyncSpecificationsWithExceptions
    {
        Because of = async () =>
        {
            await Task.Delay(10);

            throw new InvalidOperationException("something went wrong");
        };

        It should_invoke_sync = () =>
        {
            throw new InvalidOperationException("something went wrong");
        };

        It should_invoke_async = async () =>
        {
            await Task.Delay(10);

            throw new InvalidOperationException("something went wrong");
        };
    }
}
