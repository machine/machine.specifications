using System.Threading.Tasks;

namespace Machine.Specifications
{
    public class AwaitResult
    {
        public AwaitResult(Task task)
        {
            AsTask = task;
        }

        public Task AsTask { get; }
    }

    public class AwaitResult<T>
    {
        public AwaitResult(Task<T> task)
        {
            AsTask = task;
        }

        public static implicit operator T(AwaitResult<T> value)
        {
            return value.AsTask.Result;
        }

        public Task<T> AsTask { get; }
    }
}
