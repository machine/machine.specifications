namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum RemoteStatus
    {
        Failing,
        Passing,
        NotImplemented,
        Ignored
    }

    [Serializable]
    public class RemoteExceptionResult
    {
        public string FullTypeName { get; private set; }
        public string TypeName { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }
        public RemoteExceptionResult InnerExceptionResult { get; set; }

        public RemoteExceptionResult()
        {
        }

        public RemoteExceptionResult(Exception exception)
            : this(exception, true)
        {
        }

        RemoteExceptionResult(Exception exception, bool outermost)
        {
#if CLEAN_EXCEPTION_STACK_TRACE
      if (outermost && exception is TargetInvocationException)
      {
        exception = exception.InnerException;
      }
#endif

            this.FullTypeName = exception.GetType().FullName;
            this.TypeName = exception.GetType().Name;
            this.Message = exception.Message;
            this.StackTrace = FilterStackTrace(exception.StackTrace);

            if (exception.InnerException != null)
            {
                this.InnerExceptionResult = new RemoteExceptionResult(exception.InnerException, false);
            }
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
    public class RemoteResult
    {
        private RemoteStatus _status;
        private IDictionary<string, IDictionary<string, string>> _supplements = new Dictionary<string, IDictionary<string, string>>();


        public RemoteResult()
        {
        }

        private RemoteResult(RemoteStatus status)
        {
            this._status = status;
        }

        private RemoteResult(Exception exception)
        {
            this._status = RemoteStatus.Failing;
            this.Exception = new RemoteExceptionResult(exception);
        }

        private RemoteResult(RemoteResult result, string supplementName, IDictionary<string, string> supplement)
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
            get { return this._status == RemoteStatus.Passing; }
        }

        public RemoteExceptionResult Exception { get; set; }

        public RemoteStatus Status
        {
            get { return this._status; }
            set { this._status = value; }
        }

        public static RemoteResult Pass()
        {
            return new RemoteResult(RemoteStatus.Passing);
        }

        public static RemoteResult Ignored()
        {
            return new RemoteResult(RemoteStatus.Ignored);
        }

        public static RemoteResult NotImplemented()
        {
            return new RemoteResult(RemoteStatus.NotImplemented);
        }

        public static RemoteResult Failure(Exception exception)
        {
            return new RemoteResult(exception);
        }

        public static RemoteResult ContextFailure(Exception exception)
        {
            return new RemoteResult(exception);
        }

        public static RemoteResult Supplement(RemoteResult result, string supplementName, IDictionary<string, string> supplement)
        {
            return new RemoteResult(result, supplementName, supplement);
        }


    }
}