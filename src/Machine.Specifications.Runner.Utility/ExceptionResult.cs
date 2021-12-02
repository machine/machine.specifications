using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class ExceptionResult
    {
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
            FullTypeName = fullTypeName;
            TypeName = typeName;
            Message = message;
            StackTrace = stackTrace;
            InnerExceptionResult = innerExceptionResult;
        }

        public string FullTypeName { get; private set; }

        public string TypeName { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }

        public ExceptionResult InnerExceptionResult { get; set; }

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
        
        public static ExceptionResult Parse(string exceptionResultXml)
        {
            if (string.IsNullOrEmpty(exceptionResultXml))
            {
                return null;
            }

            var document = XDocument.Parse(exceptionResultXml);
            var exceptionResult = document.Element("exceptionresult");

            if (exceptionResult == null || exceptionResult.IsEmpty)
            {
                return null;
            }

            var fullTypeName = document.SafeGet<string>("/exceptionresult/fulltypename");
            var typename = document.SafeGet<string>("/exceptionresult/typename");
            var message = document.SafeGet<string>("/exceptionresult/message");
            var stacktrace = document.SafeGet<string>("/exceptionresult/stacktrace");
            var inner = document.XPathSelectElement("/exceptionresult/innerexceptionresult/exceptionresult");

            ExceptionResult innerExceptionResult = null;

            if (inner != null)
            {
                innerExceptionResult = Parse(inner.ToString());
            }
            
            return new ExceptionResult(fullTypeName, typename, message, stacktrace, innerExceptionResult);
        }
    }
}
