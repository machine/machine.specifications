using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Machine.Specifications
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class ExceptionResult
    {
        public ExceptionResult(Exception exception)
            : this(exception, true)
        {
        }

        private ExceptionResult(Exception exception, bool outermost)
        {
            if (outermost && exception is TargetInvocationException)
            {
                exception = exception.InnerException;
            }

            FullTypeName = exception.GetType().FullName;
            TypeName = exception.GetType().Name;
            Message = exception.Message;
            StackTrace = FilterStackTrace(exception.StackTrace);

            if (exception.InnerException != null)
            {
                InnerExceptionResult = new ExceptionResult(exception.InnerException, false);
            }
        }

        public string FullTypeName { get; private set; }

        public string TypeName { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }

        public ExceptionResult InnerExceptionResult { get; private set; }

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
        private static readonly Regex FrameworkStackLine = new Regex("^\\s+\\w+\\sMachine\\.Specifications\\.", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Filters the stack trace to remove all lines that occur within the testing framework.
        /// </summary>
        /// <param name="stackTrace">The original stack trace</param>
        /// <returns>The filtered stack trace</returns>
        private static string FilterStackTrace(string stackTrace)
        {
            if (stackTrace == null)
            {
                return null;
            }

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

        #endregion
    }
}
