using System.IO;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    internal static class AssemblyPathExtensions
    {
        public static XElement ToXml(this AssemblyPath path)
        {
            return new XElement("assembly", Path.GetFullPath(path));
        }
    }
}