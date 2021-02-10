using System.Threading.Tasks;
using Machine.Specifications;

#if !NET45
namespace Example.Random
{
    public class AsyncSpecificationsValueTask
    {
        public static int establish_value;

        public static int because_value;

        public static int async_it_value;

        public static int sync_it_value;

        public static int cleanup_value;

        public static ValueTask<int> Test()
        {
            return new ValueTask<int>(10);
        }

        Establish context = async () =>
            establish_value = await Test();

        Because of = async () =>
            because_value = await Test();

        It should_invoke_sync = () =>
            sync_it_value = Test().Result;

        It should_invoke_async = async () =>
            async_it_value = await Test();

        Cleanup after = async () =>
            cleanup_value = await Test();
    }
}
#endif
