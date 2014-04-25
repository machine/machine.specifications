namespace Machine.Specifications
{
    using System.Xml.Linq;

    internal static class ExceptionResultExtensions
    {
        public static string ToXml(this ExceptionResult exceptionResult)
        {
            return exceptionResult
                .ToXmlInternal()
                .ToString();
        }

        internal static XElement ToXmlInternal(this ExceptionResult exceptionResult)
        {
            if (exceptionResult == null)
            {
                return new XElement("exceptionresult");
            }

            return new XElement(
                "exceptionresult",
                new XElement("fulltypename", exceptionResult.FullTypeName),
                new XElement("typename", exceptionResult.TypeName),
                new XElement("message", exceptionResult.Message),
                new XElement("stacktrace", exceptionResult.StackTrace),
                new XElement("innerexceptionresult", exceptionResult.InnerExceptionResult != null ? ToXmlInternal(exceptionResult.InnerExceptionResult) : null));
        }
    }
}