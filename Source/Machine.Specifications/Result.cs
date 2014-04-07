using System;
using System.Collections.Generic;
using System.Text;

#if CLEAN_EXCEPTION_STACK_TRACE
using System.Text.RegularExpressions;
#endif

namespace Machine.Specifications
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
        public ExceptionResult InnerExceptionResult { get; private set; }

        public ExceptionResult(Exception exception)
            : this(exception, true)
        {
        }

        ExceptionResult(Exception exception, bool outermost)
        {
#if CLEAN_EXCEPTION_STACK_TRACE
      if (outermost && exception is TargetInvocationException)
      {
        exception = exception.InnerException;
      }
#endif

            FullTypeName = exception.GetType().FullName;
            TypeName = exception.GetType().Name;
            Message = exception.Message;
            StackTrace = FilterStackTrace(exception.StackTrace);

            if (exception.InnerException != null)
            {
                InnerExceptionResult = new ExceptionResult(exception.InnerException, false);
            }
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append(FullTypeName);

            if (!string.IsNullOrEmpty(Message))
            {
                message.AppendFormat(": {0}", Message);
            }
            if (InnerExceptionResult != null)
            {
                message.AppendFormat(" ---> {0}{1}   --- End of inner exception stack trace ---", InnerExceptionResult, Environment.NewLine);
            }
            if (StackTrace != null)
            {
                message.Append(Environment.NewLine + StackTrace);
            }

            return message.ToString();
        }

        #region Borrowed from XUnit to clean up the stack trace, licened under MS-PL

#if CLEAN_EXCEPTION_STACK_TRACE
    /// <summary>
    ///  A description of the regular expression:
    ///
    ///  ^\s+\w+\sMachine\.Specifications
    ///      Beginning of string
    ///      Whitespace, one or more repetitions
    ///      Alphanumeric, one or more repetitions
    ///      Whitespace
    ///      Machine
    ///      Literal .
    ///      Specifications
    ///      Literal .
    /// </summary>
    static readonly Regex FrameworkStackLine = new Regex("^\\s+\\w+\\sMachine\\.Specifications\\.",
                                                         RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// Filters the stack trace to remove all lines that occur within the testing framework.
    /// </summary>
    /// <param name="stackTrace">The original stack trace</param>
    /// <returns>The filtered stack trace</returns>
    static string FilterStackTrace(string stackTrace)
    {
      if (stackTrace == null)
        return null;

      var lines = stackTrace
        .Split(new[] {Environment.NewLine}, StringSplitOptions.None)
        .Where(line => !IsFrameworkStackFrame(line))
        .ToArray();

      return string.Join(Environment.NewLine, lines);
    }

    static bool IsFrameworkStackFrame(string trimmedLine)
    {
      // Anything in the Machine.Specifications namespace
      return FrameworkStackLine.IsMatch(trimmedLine);
    }
#else
        // Do not change the line at all if you are not going to clean it
        static string FilterStackTrace(string stackTrace)
        {
            return stackTrace;
        }
#endif

        #endregion
    }

    [Serializable]
    public class Result
    {
        readonly Status _status;
        readonly IDictionary<string, IDictionary<string, string>> _supplements = new Dictionary<string, IDictionary<string, string>>();

        public IDictionary<string, IDictionary<string, string>> Supplements
        {
            get { return _supplements; }
        }

        public bool Passed
        {
            get { return _status == Status.Passing; }
        }

        public ExceptionResult Exception { get; private set; }

        public Status Status
        {
            get { return _status; }
        }

        public bool HasSupplement(string name)
        {
            return _supplements.ContainsKey(name);
        }

        public IDictionary<string, string> GetSupplement(string name)
        {
            return _supplements[name];
        }

        private Result(Exception exception)
        {
            _status = Status.Failing;
            this.Exception = new ExceptionResult(exception);
        }

        private Result(Status status)
        {
            _status = status;
        }

        private Result(Result result, string supplementName, IDictionary<string, string> supplement)
        {
            _status = result.Status;
            this.Exception = result.Exception;

            foreach (var pair in result._supplements)
            {
                _supplements.Add(pair);
            }

            if (HasSupplement(supplementName))
            {
                throw new ArgumentException("Result already has supplement named: " + supplementName, "supplementName");
            }

            _supplements.Add(supplementName, supplement);
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