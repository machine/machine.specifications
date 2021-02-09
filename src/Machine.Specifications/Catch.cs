using System;
#if !NET35
using System.Threading.Tasks;
#endif

namespace Machine.Specifications
{
    public static class Catch
    {
        public static Exception Exception(Action throwingAction)
        {
            try
            {
                throwingAction();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        public static Exception Exception<T>(Func<T> throwingFunc)
        {
#if !NET35
            Task task;
#endif
            try
            {
                var result = throwingFunc();
#if !NET35
                task = result as Task;
#endif
            }
            catch (Exception exception)
            {
                return exception;
            }
#if !NET35
            if (task != null)
            {
                throw new InvalidOperationException("You must use Catch.ExceptionAsync for async methods");
            }
#endif
            return null;
        }

#if !NET35
        public static Task<Exception> ExceptionAsync(Func<Task> throwingAction)
        {
            Exception exception = null;

            try
            {
                throwingAction().Wait();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var source = new TaskCompletionSource<Exception>(exception);
            source.TrySetResult(exception);

            return source.Task;
        }
#endif

        public static TException Only<TException>(Action throwingAction)
            where TException : Exception
        {
            try
            {
                throwingAction();
            }
            catch (TException exception)
            {
                return exception;
            }

            return null;
        }
    }
}
