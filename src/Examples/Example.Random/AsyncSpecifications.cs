using System.Threading.Tasks;
using Machine.Specifications;

namespace Example.Random
{
    public class AsyncSpecifications
    {
        public static bool establish_invoked;

        public static bool because_invoked;

        public static bool async_it_invoked;

        public static bool sync_it_invoked;

        public static bool cleanup_invoked;

        Establish context = async () =>
        {
            establish_invoked = true;
            await Task.Delay(10);
        };

        Because of = async () =>
        {
            because_invoked = true;
            await Task.Delay(10);
        };

        It should_invoke_sync = () =>
            sync_it_invoked = true;

        It should_invoke_async = async () =>
        {
            async_it_invoked = true;
            await Task.Delay(10);
        };

        Cleanup after = async () =>
        {
            cleanup_invoked = true;
            await Task.Delay(10);
        };
    }
}
