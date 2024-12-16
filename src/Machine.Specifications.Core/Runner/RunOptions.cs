using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner
{
#if !NET6_0_OR_GREATER
    [Serializable]
#endif
    public class RunOptions
    {
        public IEnumerable<string> IncludeTags { get; }

        public IEnumerable<string> ExcludeTags { get; }

        public IEnumerable<string> Filters { get; }

        public IEnumerable<string> Contexts { get; }

        public RunOptions(IEnumerable<string> includeTags, IEnumerable<string> excludeTags, IEnumerable<string> filters)
            : this(includeTags, excludeTags, filters, Enumerable.Empty<string>())
        {
        }

        public RunOptions(IEnumerable<string> includeTags, IEnumerable<string> excludeTags, IEnumerable<string> filters, IEnumerable<string> contexts)
        {
            IncludeTags = includeTags;
            ExcludeTags = excludeTags;
            Filters = filters;
            Contexts = contexts;
        }

        public static RunOptions Default => new RunOptions(
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>());

        public static RunOptions Parse(string runOptionsXml)
        {
            if (runOptionsXml == null)
            {
                throw new ArgumentNullException("runOptionsXml");
            }

            var document = XDocument.Parse(runOptionsXml);

            var includeTags = Parse(document, "/runoptions/includetags/tag");
            var excludeTags = Parse(document, "/runoptions/excludetags/tag");
            var filters = Parse(document, "/runoptions/filters/filter");
            var contexts = Parse(document, "/runoptions/contexts/context");

            return new RunOptions(includeTags, excludeTags, filters, contexts);
        }

        private static IEnumerable<string> Parse(XDocument document, string xpath)
        {
            return document.XPathSelectElements(xpath).Select(supplement => supplement.Value).ToList();
        }
    }
}
