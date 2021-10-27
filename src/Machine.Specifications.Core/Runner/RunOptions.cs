using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class RunOptions
    {
        public IEnumerable<string> IncludeTags { get; private set; }
        public IEnumerable<string> ExcludeTags { get; private set; }
        public IEnumerable<string> Filters { get; private set; }
        public IEnumerable<string> Contexts { get; private set; }

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

        public static RunOptions Default { get { return new RunOptions(Enumerable.Empty<string>(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), Enumerable.Empty<string>()); } }

        public static RunOptions Parse(string runOptionsXml)
        {
            if (runOptionsXml == null)
                throw new ArgumentNullException("runOptionsXml");
            var document = XDocument.Parse(runOptionsXml);

            IEnumerable<string> includeTags = Parse(document, "/runoptions/includetags/tag");
            IEnumerable<string> excludeTags = Parse(document, "/runoptions/excludetags/tag");
            IEnumerable<string> filters = Parse(document, "/runoptions/filters/filter");
            IEnumerable<string> contexts = Parse(document, "/runoptions/contexts/context");

            return new RunOptions(includeTags, excludeTags, filters, contexts);
        }

        private static IEnumerable<string> Parse(XDocument document, string xpath)
        {
            return document.XPathSelectElements(xpath).Select(supplement => supplement.Value).ToList();
        }
    }
}
