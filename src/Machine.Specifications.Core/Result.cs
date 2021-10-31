using System;
using System.Collections.Generic;

namespace Machine.Specifications
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class Result
    {
        private Result(Exception exception)
        {
            Status = Status.Failing;
            Exception = new ExceptionResult(exception);
        }

        private Result(Status status)
        {
            Status = status;
        }

        private Result(Result result, string supplementName, IDictionary<string, string> supplement)
        {
            Status = result.Status;
            Exception = result.Exception;

            foreach (var pair in result.Supplements)
            {
                Supplements.Add(pair);
            }

            if (HasSupplement(supplementName))
            {
                throw new ArgumentException($"Result already has supplement named: {supplementName}", nameof(supplementName));
            }

            Supplements.Add(supplementName, supplement);
        }

        public IDictionary<string, IDictionary<string, string>> Supplements { get; } = new Dictionary<string, IDictionary<string, string>>();

        public bool Passed => Status == Status.Passing;

        public ExceptionResult Exception { get; private set; }

        public Status Status { get; }

        public bool HasSupplement(string name)
        {
            return Supplements.ContainsKey(name);
        }

        public IDictionary<string, string> GetSupplement(string name)
        {
            return Supplements[name];
        }

        public static Result Pass()
        {
            return new Result(Status.Passing);
        }

        public static Result Ignored()
        {
            return new Result(Status.Ignored);
        }

        public static Result NotImplemented()
        {
            return new Result(Status.NotImplemented);
        }

        public static Result Failure(Exception exception)
        {
            return new Result(exception);
        }

        public static Result ContextFailure(Exception exception)
        {
            return new Result(exception);
        }

        public static Result Supplement(Result result, string supplementName, IDictionary<string, string> supplement)
        {
            return new Result(result, supplementName, supplement);
        }
    }
}
