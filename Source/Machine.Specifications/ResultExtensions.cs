namespace Machine.Specifications
{
    using System.Collections.Generic;
    using System.Xml.Linq;

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

            foreach (KeyValuePair<string, IDictionary<string, string>> pair in result.Supplements)
            {
                var element = new XElement("supplement", new XAttribute("key", pair.Key));

                foreach (KeyValuePair<string, string> valuePair in pair.Value)
                {
                    element.Add(new XElement("entry", valuePair.Value, new XAttribute("key", valuePair.Key)));
                }

                root.Add(element);
            }

            return root;
        }
    }
}