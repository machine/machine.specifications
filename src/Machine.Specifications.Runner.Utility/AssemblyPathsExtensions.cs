using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    internal static class AssemblyPathsExtensions
    {
        public static XElement ToXml(this IEnumerable<AssemblyPath> paths)
        {
            return new XElement("assemblies", paths.Select(p => p.ToXml()).ToList());
        }
    }
}