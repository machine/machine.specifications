using System;
using System.Linq;
using System.Threading.Tasks;

namespace Machine.Specifications
{
    public static class TaskSpecificationExtensions
    {
        public static AwaitResult<T> Await<T>(this Task<T> task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions.Count == 1)
                {
                    throw e.InnerExceptions.First();
                }
                throw;
            }

            return new AwaitResult<T>(task);
        }

        public static AwaitResult Await(this Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions.Count == 1)
                {
                    throw e.InnerExceptions.First();
                }
                throw;
            }
            return new AwaitResult(task);
        }
    }
}