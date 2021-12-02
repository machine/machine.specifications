using System.Xml.Linq;
using Machine.Specifications.Utility;

namespace Machine.Specifications
{
    internal static class ResultExtensions
    {
        public static XElement ToXml(this Result result)
        {
            return
                new XElement(
                    "result",
                    new XElement("status", result.Status.ToString()),
                    new XElement("exception", result.Exception.ToXml()),
                    result.SupplementsToXml());
        }

        private static XElement SupplementsToXml(this Result result)
        {
            var root = new XElement("supplements");

            foreach (var (key, value) in result.Supplements)
            {
                var element = new XElement("supplement", new XAttribute("key", key));

                foreach (var (innerKey, innerValue) in value)
                {
                    element.Add(new XElement("entry", innerValue, new XAttribute("key", innerKey)));
                }

                root.Add(element);
            }

            return root;
        }
    }
}
