using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner.Utility
{
    public enum Status
    {
        Failing,
        Passing,
        NotImplemented,
        Ignored
    }

    [Serializable]
    public class ExceptionResult
    {
        public string FullTypeName { get; private set; }
        public string TypeName { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }
        public ExceptionResult InnerExceptionResult { get; set; }

        public ExceptionResult(Exception exception)
            : this(exception.GetType().FullName, exception.GetType().Name, exception.Message, exception.StackTrace, null)
        {
            if (exception.InnerException != null)
            {
                InnerExceptionResult = new ExceptionResult(exception.InnerException);
            }
        }

        public ExceptionResult(string fullTypeName, string typeName, string message, string stackTrace, ExceptionResult innerExceptionResult)
        {
            this.FullTypeName = fullTypeName;
            this.TypeName = typeName;
            this.Message = message;
            this.StackTrace = stackTrace;
            this.InnerExceptionResult = innerExceptionResult;
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append(this.FullTypeName);

            if (!string.IsNullOrEmpty(this.Message))
            {
                message.AppendFormat(": {0}", this.Message);
            }
            if (this.InnerExceptionResult != null)
            {
                message.AppendFormat(" ---> {0}{1}   --- End of inner exception stack trace ---", this.InnerExceptionResult, Environment.NewLine);
            }
            if (this.StackTrace != null)
            {
                message.Append(Environment.NewLine + this.StackTrace);
            }

            return message.ToString();
        }
        
        public static ExceptionResult Parse(string exceptionResultXml)
        {
            if (string.IsNullOrEmpty(exceptionResultXml))
            {
                return null;
            }

            var document = XDocument.Parse(exceptionResultXml);
            var exceptionresult = document.Element("exceptionresult");

            if (exceptionresult == null || exceptionresult.IsEmpty)
            {
                return null;
            }

            var fulltypename = document.SafeGet<string>("/exceptionresult/fulltypename");
            var typename = document.SafeGet<string>("/exceptionresult/typename");
            var message = document.SafeGet<string>("/exceptionresult/message");
            var stacktrace = document.SafeGet<string>("/exceptionresult/stacktrace");
            XElement inner = document.XPathSelectElement("/exceptionresult/innerexceptionresult/exceptionresult");
            ExceptionResult innerexceptionresult = null;

            if (inner != null)
            {
                innerexceptionresult = Parse(inner.ToString());
            }
            
            return new ExceptionResult(fulltypename, typename, message, stacktrace, innerexceptionresult);
        }
    }

    [Serializable]
    public class Result
    {
        private Status _status;
        private IDictionary<string, IDictionary<string, string>> _supplements = new Dictionary<string, IDictionary<string, string>>();
        
        public Result()
        {
        }

        private Result(Status status)
        {
            this._status = status;
        }

        private Result(Exception exception)
        {
            _status = Status.Failing;
            this.Exception = new ExceptionResult(exception);
        }

        private Result(ExceptionResult exception)
        {
            _status = Status.Failing;
            this.Exception = exception;
        }

        private Result(Result result, string supplementName, IDictionary<string, string> supplement)
        {
            this._status = result.Status;
            this.Exception = result.Exception;

            foreach (var pair in result._supplements)
            {
                this._supplements.Add(pair);
            }

            if (this.HasSupplement(supplementName))
            {
                throw new ArgumentException("Result already has supplement named: " + supplementName, "supplementName");
            }

            this._supplements.Add(supplementName, supplement);
        }

        public IDictionary<string, IDictionary<string, string>> Supplements
        {
            get { return this._supplements; }
        }

        public bool HasSupplement(string name)
        {
            return this._supplements.ContainsKey(name);
        }

        public IDictionary<string, string> GetSupplement(string name)
        {
            return this._supplements[name];
        }

        public bool Passed
        {
            get { return this._status == Status.Passing; }
        }

        public ExceptionResult Exception { get; set; }

        public Status Status
        {
            get { return this._status; }
            set { this._status = value; }
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
            var exceptionresult = document.XPathSelectElement("/result/exception/exceptionresult");
            if (exceptionresult != null)
            {
                result.Exception = ExceptionResult.Parse(exceptionresult.ToString());
            }

            foreach (XElement supplement in document.XPathSelectElements("/result/supplements/supplement"))
            {
                string key = supplement.Attribute("key").Value;
                Dictionary<string, string> supplements = supplement.Elements("entry").ToDictionary(x => x.Attribute("key").Value, x => x.Value);
                result.Supplements.Add(key, supplements);
            }

            return result;
        }
    }
}
