using System.Collections.Generic;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    public static class RunOptionsExtensions
    {
        public static string ToXml(this RunOptions runOptions)
        {
            var root = new XElement("runoptions");

            AppendIncludeTags(root, runOptions);
            AppendExcludeTags(root, runOptions);
            AppendFilters(root, runOptions);

            root.Add(new XElement("shadowcopycachepath", runOptions.ShadowCopyCachePath));

            return root.ToString();
        }

        private static void AppendIncludeTags(XElement root, RunOptions runOptions)
        {
            AppendItems(root, runOptions.IncludeTags, "includetags", "tag");
        }

        private static void AppendExcludeTags(XElement root, RunOptions runOptions)
        {
            AppendItems(root, runOptions.ExcludeTags, "excludetags", "tag");
        }

        private static void AppendFilters(XElement root, RunOptions runOptions)
        {
            AppendItems(root, runOptions.Filters, "filters", "filter");
        }

        private static void AppendItems(XElement root, IEnumerable<string> items, string itemsElementName, string itemElementName)
        {
            var itemsRootNode = new XElement(itemsElementName);
            foreach (string item in items)
            {
                itemsRootNode.Add(new XElement(itemElementName, item));
            }
            root.Add(itemsRootNode);
        }
    }
}