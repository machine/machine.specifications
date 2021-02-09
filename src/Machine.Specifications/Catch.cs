using System;
using System.Threading.Tasks;

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
            Task task;

            try
            {
                task = throwingFunc() as Task;
            }
            catch (Exception exception)
            {
                return exception;
            }

            if (task != null)
            {
                throw new InvalidOperationException("You must use Catch.ExceptionAsync for async methods");
            }

            return null;
        }

        public static async Task<Exception> ExceptionAsync(Func<Task> throwingAction)
        {
            try
            {
                await throwingAction();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

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
