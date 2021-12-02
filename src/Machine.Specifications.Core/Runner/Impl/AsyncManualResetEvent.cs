using System.Threading.Tasks;

namespace Machine.Specifications.Runner.Impl
{
    internal class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();

        public AsyncManualResetEvent()
        {
            source.TrySetResult(true);
        }

        public void Reset()
        {
            if (source.Task.IsCompleted)
            {
                source = new TaskCompletionSource<bool>();
            }
        }

        public void Set()
        {
            source.TrySetResult(true);
        }

        public Task WaitAsync()
        {
            return source.Task;
        }
    }
}
