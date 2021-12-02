using System;
using System.Threading.Tasks;

namespace Machine.Specifications
{
    public static class Catch
    {
        public static Exception Exception(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        public static Exception Exception(Func<object> throwingFunc)
        {
            Task task;

            try
            {
                task = throwingFunc() as Task;
            }
            catch (Exception ex)
            {
                return ex;
            }

            if (task != null)
            {
                throw new InvalidOperationException("You must use Catch.ExceptionAsync for async methods");
            }

            return null;
        }

        public static async Task<Exception> ExceptionAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        public static async Task<Exception> ExceptionAsync(Func<ValueTask> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        public static async Task<Exception> ExceptionAsync<T>(Func<ValueTask<T>> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }
    }
}
