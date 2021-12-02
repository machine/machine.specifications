using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class Result
    {
        public Result()
        {
        }

        private Result(Status status)
        {
            Status = status;
        }

        private Result(Exception exception)
        {
            Status = Status.Failing;
            Exception = new ExceptionResult(exception);
        }

        private Result(ExceptionResult exception)
        {
            Status = Status.Failing;
            Exception = exception;
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
                throw new ArgumentException("Result already has supplement named: " + supplementName, "supplementName");
            }

            Supplements.Add(supplementName, supplement);
        }

        public IDictionary<string, IDictionary<string, string>> Supplements { get; } = new Dictionary<string, IDictionary<string, string>>();

        public bool Passed => Status == Status.Passing;

        public ExceptionResult Exception { get; set; }

        public Status Status { get; set; }

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

        public static Result Supplement(Result result, string supplementName, IDictionary<string, string> supplement)
        {
            return new Result(result, supplementName, supplement);
        }

        public static Result Failure(Exception exception)
        {
            return new Result(exception);
        }

        public static Result Failure(ExceptionResult exception)
        {
            return new Result(exception);
        }

        public static Result Parse(string resultXml)
        {
            var document = XDocument.Parse(resultXml);
            var status = document.SafeGet<Status>("/result/status");
            var result = new Result(status);
            var exceptionResult = document.XPathSelectElement("/result/exception/exceptionresult");

            if (exceptionResult != null)
            {
                result.Exception = ExceptionResult.Parse(exceptionResult.ToString());
            }

            foreach (var supplement in document.XPathSelectElements("/result/supplements/supplement"))
            {
                var key = supplement.Attribute("key")?.Value;
                var supplements = supplement.Elements("entry").ToDictionary(x => x.Attribute("key")?.Value, x => x.Value);

                if (key != null)
                {
                    result.Supplements.Add(key, supplements);
                }
            }

            return result;
        }
    }
}
