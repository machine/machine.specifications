namespace Machine.Specifications
{
    using System.Xml.Linq;

    internal static class ExceptionResultExtensions
    {
        public static XElement ToXml(this ExceptionResult exceptionResult)
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
                new XElement("innerexceptionresult", exceptionResult.InnerExceptionResult != null ? ToXml(exceptionResult.InnerExceptionResult) : null));
        }
    }
}