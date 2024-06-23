#if !NET35
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Machine.Specifications.Runner.Impl
{
    internal class AsyncSynchronizationContext : SynchronizationContext
    {
        private readonly SynchronizationContext inner;

        private readonly AsyncManualResetEvent events = new AsyncManualResetEvent();

        private int callCount;

#if !NET40
        private ExceptionDispatchInfo exception;
#else
        private Exception exception;
#endif

        public AsyncSynchronizationContext(SynchronizationContext inner)
        {
            this.inner = inner;
        }

        private void Execute(SendOrPostCallback callback, object state)
        {
            try
            {
                callback(state);
            }
            catch (Exception ex)
            {
#if !NET40
                exception = ExceptionDispatchInfo.Capture(ex);
#else
                exception = ex;
#endif
            }
            finally
            {
                OperationCompleted();
            }
        }

        public override void OperationCompleted()
        {
            var count = Interlocked.Decrement(ref callCount);

            if (count == 0)
            {
                events.Set();
            }
        }

        public override void OperationStarted()
        {
            Interlocked.Increment(ref callCount);

            events.Reset();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            OperationStarted();

            try
            {
                if (inner == null)
                {
                    ThreadPool.QueueUserWorkItem(_ => Execute(d, state));
                }
                else
                {
                    inner.Post(_ => Execute(d, state), null);
                }
            }
            catch
            {
                // ignored
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            try
            {
                if (inner == null)
                {
                    d(state);
                }
                else
                {
                    inner.Send(d, state);
                }
            }
            catch (Exception ex)
            {
#if !NET40
                exception = ExceptionDispatchInfo.Capture(ex);
#else
                exception = ex;
#endif
            }
        }

#if !NET40
        public ExceptionDispatchInfo WaitAsync()
#else
        public Exception WaitAsync()
#endif
        {
            events.Wait();

            return exception;
        }
    }
}
#endif
